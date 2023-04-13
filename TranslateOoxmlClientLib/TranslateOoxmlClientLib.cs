using System.Net;
using static System.IO.File;

namespace TranslateOoxml;

/// <summary>
/// TranslateOoxml service client library.
/// </summary>
public static class TranslateOoxmlClientLib
{
    private static readonly HttpClient HttpClient;

    static TranslateOoxmlClientLib()
    {
        HttpClient = new HttpClient();
    }

    /// <summary>
    /// Translates an OOXML document as an asynchronous operation using the TranslateOoxml service.
    /// </summary>
    /// <param name="sourcePath">The source document path.</param>
    /// <param name="targetPath">The target document path.</param>
    /// <param name="targetLanguage">The target language.</param>
    /// <param name="serviceUrl">The TranslateOoxml service URL.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="FileNotFoundException">
    /// Thrown when the source document does not exist.
    /// </exception>
    public static async Task<HttpStatusCode> TranslateDocumentAsync(
        string sourcePath,
        string targetPath,
        string targetLanguage,
        string serviceUrl)
    {
        if (!Exists(sourcePath))
            throw new FileNotFoundException(null, sourcePath);

        using var sourceStream = File.OpenRead(sourcePath);
        using var requestHttpContent = new StreamContent(sourceStream);
        using var response =
            await HttpClient.PostAsync(serviceUrl + '/' + targetLanguage, requestHttpContent)
            .ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
        {
            using var responseHttpContent = response.Content;
            using var targetStream = File.Create(targetPath);
            await responseHttpContent.CopyToAsync(targetStream).ConfigureAwait(false);
        }
        return response.StatusCode;
    }
}
