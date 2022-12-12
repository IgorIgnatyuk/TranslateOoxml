using System.IO.Compression;
using static TranslateOoxml.DeepLTranslator;
using static TranslateOoxml.OoxmlTranslator;

namespace TranslateOoxml;

internal static class TranslateOoxmlService
{
    private static void Main()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapPost(
            "/translate-ooxml/{targetLanguage}",
            async (string targetLanguage, HttpRequest request, HttpResponse response) =>
            {
                var stream = new MemoryStream();
                await request.Body.CopyToAsync(stream);

                using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Update, true))
                    await TranslateZipArchive(
                        zipArchive,
                        async (text) => await TranslateXml(text, targetLanguage));

                response.ContentType = "application/octet-stream";
                stream.Position = 0;
                await stream.CopyToAsync(response.Body);
            })
            .Accepts<IFormFile>("application/octet-stream")
            .WithName("PostTranslateOoxml");

        app.Run();
    }
}
