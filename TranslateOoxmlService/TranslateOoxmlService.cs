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
                app.Logger.LogInformation(
                    "Translating OOXML ({ContentLength} bytes) to {TargetLanguage}",
                    request.ContentLength, targetLanguage);

                try
                {
                    app.Logger.LogDebug("Copying the request body content to a memory stream");
                    var stream = new MemoryStream();
                    await request.Body.CopyToAsync(stream);

                    app.Logger.LogDebug("Opening the memory stream as a ZIP archive");
                    using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Update, true))
                    {
                        app.Logger.LogDebug("Translating the ZIP archive");
                        await TranslateZipArchive(
                            zipArchive,
                            async (text) => await TranslateXml(text, targetLanguage));
                    }
                    app.Logger.LogDebug("Setting Content-Type to application/octet-stream");
                    response.ContentType = "application/octet-stream";

                    app.Logger.LogDebug(
                        "Copying the translated ZIP archive to the response body content");
                    stream.Position = 0;
                    await stream.CopyToAsync(response.Body);
                }
                catch (InvalidDataException)
                {
                    app.Logger.LogError("Not a ZIP archive");
                    response.StatusCode = 400;
                }
                catch (UnsupportedFileFormatException ex)
                {
                    app.Logger.LogError("{ExceptionMessage}", ex.Message);
                    response.StatusCode = 400;
                }
                catch (Exception ex)
                {
                    app.Logger.LogError("Exception thrown: {ExceptionMessage}", ex.Message);
                    response.StatusCode = 500;
                }
            })
            .Accepts<IFormFile>("application/octet-stream")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName("PostTranslateOoxml");

        app.Run();
    }
}
