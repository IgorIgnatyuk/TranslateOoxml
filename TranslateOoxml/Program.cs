using System.IO.Compression;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using static System.Environment;
using static System.IO.File;
using static System.IO.Path;

using static Constants;

static async Task<string> Translate(string text, string targetLanguage)
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

static string ReadZipArchiveEntry(ZipArchiveEntry zipArchiveEntry)
{
    using var reader = new StreamReader(zipArchiveEntry.Open());
    return reader.ReadToEnd();
}

static void WriteZipArchiveEntry(ZipArchiveEntry zipArchiveEntry, string contents)
{
    using var writer = new StreamWriter(zipArchiveEntry.Open());
    writer.Write(contents);
}

static async Task TranslateZipArchiveEntry(ZipArchiveEntry entry, string targetLanguage)
{
    WriteZipArchiveEntry(
        entry,
        await Translate(ReadZipArchiveEntry(entry), targetLanguage));
}

static async Task TranslateOoxmlModifying(string path, string targetLanguage)
{
    using var zipArchive = ZipFile.Open(path, ZipArchiveMode.Update);
    switch (GetExtension(path).ToLower())
    {
        case ".docx":
            {
                var entry = zipArchive.GetEntry("word/document.xml");
                if (entry != null)
                    await TranslateZipArchiveEntry(entry, targetLanguage);
            }
            break;
        case ".pptx":
            foreach (var entry in zipArchive.Entries)
                if (entry.FullName.StartsWith("ppt/slides/slide"))
                    await TranslateZipArchiveEntry(entry, targetLanguage);
            break;
        case ".xlsx":
            {
                var entry = zipArchive.GetEntry("xl/sharedStrings.xml");
                if (entry != null)
                    await TranslateZipArchiveEntry(entry, targetLanguage);
            }
            break;
        default:
            throw new Exception("Unsupported file format");
    }
}

static async Task TranslateOoxml(string sourcePath, string targetLanguage)
{
    if (!File.Exists(sourcePath))
        throw new FileNotFoundException(null, sourcePath);

    var targetPath = Join(
        GetDirectoryName(sourcePath),
        GetFileNameWithoutExtension(sourcePath) + '_' + targetLanguage + GetExtension(sourcePath));

    Copy(sourcePath, targetPath, true);
    await TranslateOoxmlModifying(targetPath, targetLanguage);
}

if (args.Length == 2)
    try
    {
        await TranslateOoxml(args[0], args[1]);
    }
    catch (Exception e)
    {
        Console.Error.WriteLine(e.Message);
    }
else
    Console.Error.Write(
        $"Syntax: {GetFileNameWithoutExtension(ProcessPath)} sourceFile targetLanguage\n\n" +
        "The source file can be a .docx, .pptx, or .xlsx one.\n\n" +
        "The target file will appear in the same folder where the source file resides.\n" +
        "The target file name will have the target language as a suffix.\n\n" +
        $"The environment variable {DeepLAuthKey} should be set.\n");
