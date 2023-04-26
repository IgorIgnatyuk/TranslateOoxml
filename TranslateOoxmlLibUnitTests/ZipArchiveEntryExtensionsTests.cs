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
        await entry.TranslateAsync(
            async (text, cancellationToken) => await Task.FromResult(text.ToUpper()));
        var s = await entry.ReadAsync();
        Assert.AreEqual(s, "TEST STRING");
    }

    [TestMethod]
    public async Task Test_Cancelling_TranslateAsync()
    {
        using var stream = new MemoryStream();
        using var archive = new ZipArchive(stream, ZipArchiveMode.Update);
        var entry = archive.CreateEntry("TestEntry");
        await entry.WriteAsync("Test string");

        using var cts = new CancellationTokenSource();
        var task = entry.TranslateAsync(
            async (text, cancellationToken) =>
            {
                await Task.Delay(100, cancellationToken);
                return await Task.FromResult(text.ToUpper());
            },
            cts.Token);

        await Task.Delay(1);
        cts.Cancel();
        await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () => await task);
    }
}
