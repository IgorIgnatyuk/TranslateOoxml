﻿using System.IO.Compression;

namespace TranslateOoxml.ZipArchiveEntryExtensions;

/// <summary>
/// Translation extensions for System.IO.Compression.ZipArchiveEntry.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Reads the content of a ZipArchiveEntry.
    /// </summary>
    /// <param name="zipArchiveEntry">The ZipArchiveEntry.</param>
    /// <returns>The content of the ZipArchiveEntry as a string.</returns>
    public static string Read(this ZipArchiveEntry zipArchiveEntry)
    {
        using var stream = zipArchiveEntry.Open();
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Writes a text to the content of a ZipArchiveEntry.
    /// </summary>
    /// <param name="zipArchiveEntry">The ZipArchiveEntry.</param>
    /// <param name="contents">The text to be written.</param>
    public static void Write(this ZipArchiveEntry zipArchiveEntry, string contents)
    {
        using var stream = zipArchiveEntry.Open();
        using var writer = new StreamWriter(stream);
        writer.Write(contents);
    }

    /// <summary>
    /// Translates the content of a ZipArchiveEntry using a callback to translate text.
    /// </summary>
    /// <param name="entry">The ZipArchiveEntry.</param>
    /// <param name="translate">The callback used for text translation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public static async Task Translate(
        this ZipArchiveEntry entry,
        Func<string, Task<string>> translate)
    {
        entry.Write(await translate(entry.Read()));
    }
}
