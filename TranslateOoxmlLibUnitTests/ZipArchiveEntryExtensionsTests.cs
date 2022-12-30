using System.IO.Compression;
using static TranslateOoxml.Extensions.ZipArchiveEntryExtensions;

namespace TranslateOoxml.Test;

[TestClass]
public class ZipArchiveEntryExtensionsTests
{
    [TestMethod]
    public async Task Test_Write_Read()
    {
        using var stream = new MemoryStream();
        using var archive = new ZipArchive(stream, ZipArchiveMode.Update);
        var entry = archive.CreateEntry("TestEntry");
        await entry.Write("Test string");
        var s = await entry.Read();
        Assert.AreEqual(s, "Test string");
    }

    [TestMethod]
    public async Task Test_Write_Translate_Read()
    {
        using var stream = new MemoryStream();
        using var archive = new ZipArchive(stream, ZipArchiveMode.Update);
        var entry = archive.CreateEntry("TestEntry");
        await entry.Write("Test string");
        await entry.Translate(async (text) => await Task.FromResult(text.ToUpper()));
        var s = await entry.Read();
        Assert.AreEqual(s, "TEST STRING");
    }
}
