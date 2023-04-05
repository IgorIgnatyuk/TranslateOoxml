using System.Net.Http.Headers;
using System.Net.Http.Json;
using static System.Environment;
using static TranslateOoxml.Constants;

namespace TranslateOoxml;

/// <summary>
/// Text translator using DeepL API.
/// </summary>
public static class DeepLTranslator
{
    private static readonly HttpClient HttpClient;

    static DeepLTranslator()
    {
        HttpClient = new HttpClient();
    }

    /// <summary>
    /// Exception thrown by the DeepLTranslator methods.
    /// </summary>
    public class DeepLTranslatorException : Exception
    {
        public DeepLTranslatorException(string message) : base(message) { }
    }

    /// <summary>
    /// Translates a text as an asynchronous operation.
    /// </summary>
    /// <param name="text">The text to translate.</param>
    /// <param name="targetLanguage">The target language.</param>
    /// <returns>
    /// The task object representing the asynchronous operation that returns the translated text.
    /// </returns>
    /// <exception cref="Exception">
    /// Thrown when the environment variable DEEPL_AUTH_KEY is not set.
    /// </exception>
    public static async Task<string> TranslateXmlAsync(string text, string targetLanguage)
    {
        var deepLAuthKey = GetEnvironmentVariable(DeepLAuthKey);
        if (deepLAuthKey == null)
            throw new DeepLTranslatorException("Environment variable DEEPL_AUTH_KEY is not set");

        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("DeepL-Auth-Key", deepLAuthKey);

        using var httpContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("text", text),
            new KeyValuePair<string, string>("target_lang", targetLanguage),
            new KeyValuePair<string, string>("tag_handling", "xml")
        });
        using var response =
            await HttpClient.PostAsync("https://api-free.deepl.com/v2/translate", httpContent);

        using var responseHttpContent = response.Content;
        var result = await responseHttpContent.ReadFromJsonAsync<TranslateResult>();
        if (result != null)
            return result.Translations[0].Text;
        else
            throw new DeepLTranslatorException("Unexpected result");
    }
}
