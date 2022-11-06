using System.IO.Compression;

namespace ZipArchiveEntryExtensions;

static class Extensions
{
    public static string Read(this ZipArchiveEntry zipArchiveEntry)
    {
        using var stream = zipArchiveEntry.Open();
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public static void Write(this ZipArchiveEntry zipArchiveEntry, string contents)
    {
        using var stream = zipArchiveEntry.Open();
        using var writer = new StreamWriter(stream);
        writer.Write(contents);
    }

}
