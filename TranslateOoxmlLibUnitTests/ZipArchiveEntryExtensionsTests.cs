using System.IO.Compression;
using static TranslateOoxml.Extensions.ZipArchiveEntryExtensions;

namespace TranslateOoxml.Test;

[TestClass]
public class ZipArchiveEntryExtensionsTests
{
    [TestMethod]
    public async Task Test_WriteAsync_ReadAsync()
    {
        using var stream = new MemoryStream();
        using var archive = new ZipArchive(stream, ZipArchiveMode.Update);
        var entry = archive.CreateEntry("TestEntry");
        await entry.WriteAsync("Test string");
        var s = await entry.ReadAsync();
        Assert.AreEqual(s, "Test string");
    }

    [TestMethod]
    public async Task Test_WriteAsync_TranslateAsync_ReadAsync()
    {
        using var stream = new MemoryStream();
        using var archive = new ZipArchive(stream, ZipArchiveMode.Update);
        var entry = archive.CreateEntry("TestEntry");
        await entry.WriteAsync("Test string");
        await entry.TranslateAsync(async (text) => await Task.FromResult(text.ToUpper()));
        var s = await entry.ReadAsync();
        Assert.AreEqual(s, "TEST STRING");
    }
}
