using System.IO.Compression;
using static TranslateOoxml.DeepLTranslator;
using static TranslateOoxml.OoxmlTranslator;

namespace TranslateOoxml;

internal static class TranslateOoxmlService
{
    private static void Main()
    {
        var app = WebApplication.CreateBuilder().Build();

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

                stream.Position = 0;
                await stream.CopyToAsync(response.Body);
            });

        app.Run();
    }
}
