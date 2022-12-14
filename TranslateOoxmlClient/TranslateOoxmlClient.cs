﻿using static System.Environment;
using static System.IO.File;
using static System.IO.Path;

namespace TranslateOoxmlClient;

internal static class TranslateOoxmlClient
{
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

    private static async Task Main(string[] args)
    {
        if (args.Length == 3)
            try
            {
                var sourcePath = args[0];
                var targetLanguage = args[1];
                var serviceUrl = args[2];
                var targetPath = Join(
                    GetDirectoryName(sourcePath),
                    GetFileNameWithoutExtension(sourcePath) + '_' + targetLanguage +
                    GetExtension(sourcePath));

                await TranslateDocument(sourcePath, targetPath, targetLanguage, serviceUrl);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        else
            Console.Error.Write(
                "Syntax: " +
                $"{GetFileNameWithoutExtension(ProcessPath)} " +
                "sourceFile targetLanguageCode serviceUrl\n\n" +
                "The source file can be a .docx, .pptx, or .xlsx one.\n\n" +
                "The target file will appear in the same folder where the source file resides.\n" +
                "The target file name will have the target language code as a suffix.\n\n" +
                "Language codes: " +
                "https://www.deepl.com/docs-api/translate-text/translate-text/\n");
    }
}
