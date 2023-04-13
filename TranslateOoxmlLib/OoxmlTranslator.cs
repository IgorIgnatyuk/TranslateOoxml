using System.IO.Compression;
using static System.IO.File;
using static TranslateOoxml.Extensions.ZipArchiveEntryExtensions;

namespace TranslateOoxml;

/// <summary>
/// OOXML document translator using a callback to translate text.
/// </summary>
public static class OoxmlTranslator
{
    /// <summary>
    /// Checks if an OOXML ZipArchive is a DOCX one, and translates it if so.
    /// </summary>
    /// <param name="zipArchive">The OOXML ZipArchive.</param>
    /// <param name="translate">The callback used for text translation.</param>
    /// <returns><c>true</c> if the ZipArchive is a DOCX one; otherwise, <c>false</c>.</returns>
    public static async Task<bool> TranslateDocxZipArchiveAsync(
        ZipArchive zipArchive,
        Func<string, Task<string>> translate)
    {
        var entry = zipArchive.GetEntry("word/document.xml");
        if (entry == null)
            return false;

        await entry.TranslateAsync(translate).ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Checks if an OOXML ZipArchive is a PPTX one, and translates it if so.
    /// </summary>
    /// <param name="zipArchive">The OOXML ZipArchive.</param>
    /// <param name="translate">The callback used for text translation.</param>
    /// <returns><c>true</c> if the ZipArchive is a PPTX one; otherwise, <c>false</c>.</returns>
    public static async Task<bool> TranslatePptxZipArchiveAsync(
        ZipArchive zipArchive,
        Func<string, Task<string>> translate)
    {
        var slideFound = false;
        foreach (var entry in zipArchive.Entries)
            if (entry.FullName.StartsWith("ppt/slides/slide"))
            {
                slideFound = true;
                await entry.TranslateAsync(translate).ConfigureAwait(false);
            }

        return slideFound;
    }

    /// <summary>
    /// Checks if an OOXML ZipArchive is a XLSX one, and translates it if so.
    /// </summary>
    /// <param name="zipArchive">The OOXML ZipArchive.</param>
    /// <param name="translate">The callback used for text translation.</param>
    /// <returns><c>true</c> if the ZipArchive is a XLSX one; otherwise, <c>false</c>.</returns>
    public static async Task<bool> TranslateXlsxZipArchiveAsync(
        ZipArchive zipArchive,
        Func<string, Task<string>> translate)
    {
        var entry = zipArchive.GetEntry("xl/sharedStrings.xml");
        if (entry == null)
            return false;

        await entry.TranslateAsync(translate).ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Exception thrown when the format of the document to translate is unsupported.
    /// </summary>
    public class UnsupportedFileFormatException : Exception
    {
        public UnsupportedFileFormatException() : base("Unsupported file format") { }
    }

    /// <summary>
    /// Translates an OOXML ZipArchive as an asynchronous operation.
    /// </summary>
    /// <param name="zipArchive">
    /// The OOXML ZipArchive to translate (opened in ZipArchiveMode.Update).
    /// </param>
    /// <param name="translate">The callback used for text translation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="UnsupportedFileFormatException">
    /// Thrown when the source document format is not supported.
    /// </exception>
    public static async Task TranslateZipArchiveAsync(
        ZipArchive zipArchive,
        Func<string, Task<string>> translate)
    {
        if (
            !await TranslateDocxZipArchiveAsync(zipArchive, translate).ConfigureAwait(false) &&
            !await TranslatePptxZipArchiveAsync(zipArchive, translate).ConfigureAwait(false) &&
            !await TranslateXlsxZipArchiveAsync(zipArchive, translate).ConfigureAwait(false))

            throw new UnsupportedFileFormatException();
    }

    /// <summary>
    /// Translates an OOXML document as an asynchronous operation.
    /// </summary>
    /// <param name="sourcePath">The source document path.</param>
    /// <param name="targetPath">The target document path.</param>
    /// <param name="translate">The callback used for text translation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="FileNotFoundException">
    /// Thrown when the source document does not exist.
    /// </exception>
    /// <exception cref="UnsupportedFileFormatException">
    /// Thrown when the source document format is not supported.
    /// </exception>
    public static async Task TranslateDocumentAsync(
        string sourcePath,
        string targetPath,
        Func<string, Task<string>> translate)
    {
        if (!Exists(sourcePath))
            throw new FileNotFoundException(null, sourcePath);

        Copy(sourcePath, targetPath, true);
        try
        {
            using var zipArchive = ZipFile.Open(targetPath, ZipArchiveMode.Update);
            await TranslateZipArchiveAsync(zipArchive, translate).ConfigureAwait(false);
        }
        catch (InvalidDataException)
        {
            throw new UnsupportedFileFormatException();
        }
    }
}
