using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO.Compression;
using static TranslateOoxml.DeepLTranslator;
using static TranslateOoxml.OoxmlTranslator;

namespace TranslateOoxmlServiceLib;

/// <summary>
/// TranslateOoxml service library.
/// </summary>
public static class TranslateOoxmlServiceLib
{
    /// <summary>
    /// Processes a HTTP request to the TranslateOoxml service.
    /// </summary>
    /// <param name="targetLanguage">The target language.</param>
    /// <param name="request">The HTTP request.</param>
    /// <param name="response">The HTTP response.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public static async Task ProcessPostTranslateOoxml(
        string targetLanguage,
        HttpRequest request,
        HttpResponse response,
        ILogger logger)
    {
        logger.LogInformation(
            "Translating OOXML ({ContentLength} bytes) to {TargetLanguage}",
            request.ContentLength, targetLanguage);

        try
        {
            logger.LogDebug("Copying the request body content to a memory stream");
            var stream = new MemoryStream();
            await request.Body.CopyToAsync(stream);

            logger.LogDebug("Opening the memory stream as a ZIP archive");
            using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Update, true))
            {
                logger.LogDebug("Translating the ZIP archive");
                await TranslateZipArchive(
                    zipArchive,
                    async (text) => await TranslateXml(text, targetLanguage));
            }
            logger.LogDebug("Setting Content-Type to application/octet-stream");
            response.ContentType = "application/octet-stream";

            logger.LogDebug(
                "Copying the translated ZIP archive to the response body content");
            stream.Position = 0;
            await stream.CopyToAsync(response.Body);
        }
        catch (InvalidDataException)
        {
            logger.LogError("Not a ZIP archive");
            response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
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
    }
}
