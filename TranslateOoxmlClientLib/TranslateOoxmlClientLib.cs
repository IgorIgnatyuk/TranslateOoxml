using static System.IO.File;

namespace TranslateOoxmlClient;

/// <summary>
/// TranslateOoxml service client library.
/// </summary>
public static class TranslateOoxmlClientLib
{
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
    public static async Task TranslateDocument(
        string sourcePath,
        string targetPath,
        string targetLanguage,
        string serviceUrl)
    {
        if (!Exists(sourcePath))
            throw new FileNotFoundException(null, sourcePath);

        var client = new HttpClient();

        using var sourceStream = File.OpenRead(sourcePath);
        using var requestHttpContent = new StreamContent(sourceStream);
        using var response =
            await client.PostAsync(serviceUrl + '/' + targetLanguage, requestHttpContent);

        using var responseHttpContent = response.Content;
        using var targetStream = File.Create(targetPath);
        await responseHttpContent.CopyToAsync(targetStream);
    }
}
