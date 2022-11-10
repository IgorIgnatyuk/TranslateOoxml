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
    /// <summary>
    /// Translates a text.
    /// </summary>
    /// <param name="text">The text to translate.</param>
    /// <param name="targetLanguage">The target language.</param>
    /// <returns>The translated text.</returns>
    /// <exception cref="Exception">
    /// Thrown when the environment variable DEEPL_AUTH_KEY is not set.
    /// </exception>
    public static async Task<string> Translate(string text, string targetLanguage)
    {
        var deepLAuthKey = GetEnvironmentVariable(DeepLAuthKey);
        if (deepLAuthKey == null)
            throw new Exception("Environment variable DEEPL_AUTH_KEY is not set");

        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("DeepL-Auth-Key", deepLAuthKey);

        using var httpContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("text", text),
            new KeyValuePair<string, string>("target_lang", targetLanguage),
            new KeyValuePair<string, string>("tag_handling", "xml")
        });
        using var response =
            await client.PostAsync("https://api-free.deepl.com/v2/translate", httpContent);

        using var responseHttpContent = response.Content;
        var result = await responseHttpContent.ReadFromJsonAsync<TranslateResult>();
        if (result != null)
            return result.Translations[0].Text;
        else
            throw new Exception("Unexpected result");
    }
}
