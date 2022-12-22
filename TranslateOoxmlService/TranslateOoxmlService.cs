using static TranslateOoxmlServiceLib.TranslateOoxmlServiceLib;

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
            async (
                string targetLanguage,
                HttpRequest request,
                HttpResponse response)
                =>
            await ProcessPostTranslateOoxml(
                targetLanguage,
                request,
                response,
                app.Logger)
            )
            .Accepts<IFormFile>("application/octet-stream")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status415UnsupportedMediaType)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName("PostTranslateOoxml");

        app.Run();
    }
}
