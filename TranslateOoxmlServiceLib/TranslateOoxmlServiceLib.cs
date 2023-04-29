using System.IO.Compression;
using static TranslateOoxml.DeepLTranslator;
using static TranslateOoxml.OoxmlTranslator;

namespace TranslateOoxml;

/// <summary>
/// TranslateOoxml service library.
/// </summary>
public static class TranslateOoxmlServiceLib
{
    /// <summary>
    /// Processes a HTTP request to the TranslateOoxml service.
    /// </summary>
    /// <param name="targetLanguage">The target language.</param>
    /// <param name="requestBody">The HTTP request body.</param>
    /// <param name="responseBody">The HTTP response body.</param>
    /// <param name="log">The logging delegate.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <exception cref="UnsupportedFileFormatException">
    /// Thrown when the source document format is not supported.
    /// </exception>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public static async Task ProcessPostTranslateOoxmlAsync(
        string targetLanguage,
        Stream requestBody,
        Stream responseBody,
        Action<string> log,
        CancellationToken cancellationToken = default)
    {
        log("Copying the request body content to a memory stream");
        var stream = new MemoryStream();
        await requestBody.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);

        try
        {
            log("Opening the memory stream as a ZIP archive");
            using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Update, true);

            log("Translating the ZIP archive");
            await TranslateZipArchiveAsync(
                zipArchive,
                async (text) => await TranslateXmlAsync(
                    text,
                    targetLanguage)
                .ConfigureAwait(false)).ConfigureAwait(false);
        }
        catch (InvalidDataException)
        {
            throw new UnsupportedFileFormatException();
        }

        log("Copying the translated ZIP archive to the response body content");
        stream.Position = 0;
        await stream.CopyToAsync(responseBody, cancellationToken).ConfigureAwait(false);
    }
}
