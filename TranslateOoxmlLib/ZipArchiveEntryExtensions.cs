using System.IO.Compression;

namespace TranslateOoxml.Extensions;

/// <summary>
/// Translation extensions for System.IO.Compression.ZipArchiveEntry.
/// </summary>
public static class ZipArchiveEntryExtensions
{
    /// <summary>
    /// Reads the content of a ZipArchiveEntry.
    /// </summary>
    /// <param name="zipArchiveEntry">The ZipArchiveEntry.</param>
    /// <returns>The content of the ZipArchiveEntry as a string.</returns>
    public static async Task<string> ReadAsync(this ZipArchiveEntry zipArchiveEntry)
    {
        using var stream = zipArchiveEntry.Open();
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Writes a text to the content of a ZipArchiveEntry.
    /// </summary>
    /// <param name="zipArchiveEntry">The ZipArchiveEntry.</param>
    /// <param name="contents">The text to be written.</param>
    public static async Task WriteAsync(this ZipArchiveEntry zipArchiveEntry, string contents)
    {
        using var stream = zipArchiveEntry.Open();
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync(contents).ConfigureAwait(false);
    }

    /// <summary>
    /// Translates the content of a ZipArchiveEntry as an asynchronous operation.
    /// Uses a callback to translate text.
    /// </summary>
    /// <param name="entry">The ZipArchiveEntry.</param>
    /// <param name="translate">The callback used for text translation.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public static async Task TranslateAsync(
        this ZipArchiveEntry entry,
        Func<string, CancellationToken, Task<string>> translate,
        CancellationToken cancellationToken = default)
    {
        await entry.WriteAsync(
            await translate(await entry.ReadAsync().ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false))
            .ConfigureAwait(false);
    }
}
