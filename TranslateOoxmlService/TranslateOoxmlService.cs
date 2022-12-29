using static TranslateOoxml.OoxmlTranslator;
using static TranslateOoxml.TranslateOoxmlServiceLib;

namespace TranslateOoxml;

internal static class Program
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
            async (
                string targetLanguage,
                HttpRequest request,
                HttpResponse response) =>
            {
                var logger = app.Logger;
                logger.LogInformation(
                    "Translating OOXML ({ContentLength} bytes) to {TargetLanguage}",
                    request.ContentLength, targetLanguage);

                try
                {
                    response.ContentType = "application/octet-stream";
                    await ProcessPostTranslateOoxml(
                        targetLanguage,
                        request.Body,
                        response.Body,
                        message => logger.LogDebug("{Message}", message));
                }
                catch (UnsupportedFileFormatException ex)
                {
                    logger.LogError("{ExceptionMessage}", ex.Message);
                    response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                }
                catch (Exception ex)
                {
                    logger.LogError("Exception thrown: {ExceptionMessage}", ex.Message);
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                }
            })
            .Accepts<IFormFile>("application/octet-stream")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status415UnsupportedMediaType)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName("PostTranslateOoxml");

        app.Run();
    }
}
