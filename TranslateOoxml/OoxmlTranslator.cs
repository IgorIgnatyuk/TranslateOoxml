using System.IO.Compression;
using static System.IO.File;
using static System.IO.Path;
using static TranslateOoxml.ZipArchiveEntryExtensions.Extensions;

namespace TranslateOoxml
{
    public static class OoxmlTranslator
    {
        public static async Task TranslateDocument(
            string sourcePath,
            string targetPath,
            Func<string, Task<string>> translate)
        {
            if (!Exists(sourcePath))
                throw new FileNotFoundException(null, sourcePath);

            Copy(sourcePath, targetPath, true);
            using var zipArchive = ZipFile.Open(targetPath, ZipArchiveMode.Update);
            switch (GetExtension(targetPath).ToLower())
            {
                case ".docx":
                    {
                        var entry = zipArchive.GetEntry("word/document.xml");
                        if (entry != null)
                            await entry.Translate(translate);
                    }
                    break;
                case ".pptx":
                    foreach (var entry in zipArchive.Entries)
                        if (entry.FullName.StartsWith("ppt/slides/slide"))
                            await entry.Translate(translate);
                    break;
                case ".xlsx":
                    {
                        var entry = zipArchive.GetEntry("xl/sharedStrings.xml");
                        if (entry != null)
                            await entry.Translate(translate);
                    }
                    break;
                default:
                    throw new Exception("Unsupported file format");
            }
        }
    }
}
