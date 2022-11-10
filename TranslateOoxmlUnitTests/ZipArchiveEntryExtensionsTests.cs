using System.IO.Compression;
using static TranslateOoxml.ZipArchiveEntryExtensions.Extensions;

namespace TranslateOoxmlUnitTests;

[TestClass]
public class ZipArchiveEntryExtensionsTests
{
    [TestMethod]
    public void Test_Write_Read()
    {
        using var stream = new MemoryStream();
        using var archive = new ZipArchive(stream, ZipArchiveMode.Update);
        var entry = archive.CreateEntry("TestEntry");
        entry.Write("Test string");
        var s = entry.Read();
        Assert.AreEqual(s, "Test string");
    }

    [TestMethod]
    public void Test_Write_Translate_Read()
    {
        using var stream = new MemoryStream();
        using var archive = new ZipArchive(stream, ZipArchiveMode.Update);
        var entry = archive.CreateEntry("TestEntry");
        entry.Write("Test string");
        entry.Translate((text) => Task.FromResult(text.ToUpper())).Wait();
        var s = entry.Read();
        Assert.AreEqual(s, "TEST STRING");
    }
}
