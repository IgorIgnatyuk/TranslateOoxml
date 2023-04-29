using System.Net;
using static TranslateOoxml.Test.Helpers.Equality;
using static TranslateOoxml.Test.TestDirectories;
using static TranslateOoxml.TranslateOoxmlClientLib;

namespace TranslateOoxml.Test;

[TestClass]
public class TranslateOoxmlClientLibTests
{
    private static async Task<HttpStatusCode> Test_TranslateDocumentAsync(string filename)
    {
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        return await TranslateDocumentAsync(
            inputDir + filename,
            outputDir + filename,
            "DE",
            "https://localhost:7261/translate-ooxml");
    }

    private static async Task Test_TranslateDocumentAsync(
        string filename,
        HttpStatusCode expectedStatusCode)
    {
        var statusCode = await Test_TranslateDocumentAsync(filename);
        Assert.AreEqual(statusCode, expectedStatusCode);
        if (statusCode == HttpStatusCode.OK)
            Assert.IsTrue(FilesAreEqual(outputDir + filename, expectedOutputDir + filename));

        File.Delete(outputDir + filename);
    }

    [TestMethod]
    public async Task Test_Docx_TranslateDocumentAsync()
    {
        await Test_TranslateDocumentAsync("Test.docx", HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task Test_Pptx_TranslateDocumentAsync()
    {
        await Test_TranslateDocumentAsync("Test.pptx", HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task Test_Xlsx_TranslateDocumentAsync()
    {
        await Test_TranslateDocumentAsync("Test.xlsx", HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task Test_BadRequest_Zip_TranslateDocumentAsync()
    {
        await Test_TranslateDocumentAsync("Test.zip", HttpStatusCode.UnsupportedMediaType);
    }

    [TestMethod]
    public async Task Test_BadRequest_Txt_TranslateDocumentAsync()
    {
        await Test_TranslateDocumentAsync("Test.txt", HttpStatusCode.UnsupportedMediaType);
    }

    [TestMethod]
    public async Task Test_FileNotFound_Html_TranslateDocumentAsync()
    {
        await Assert.ThrowsExceptionAsync<FileNotFoundException>(
            async () => await Test_TranslateDocumentAsync("Test.html"));
    }

    private static async Task Test_Cancelling_TranslateDocumentAsync(
        string filename)
    {
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        using var cts = new CancellationTokenSource();
        var task = TranslateDocumentAsync(
            inputDir + filename,
            outputDir + filename,
            "DE",
            "https://localhost:7261/translate-ooxml",
            cts.Token);

        await Task.Delay(10);
        cts.Cancel();
        await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () => await task);

        File.Delete(outputDir + filename);

    }

    [TestMethod]
    public async Task Test_Docx_Cancelling_TranslateDocumentAsync()
    {
        await Test_Cancelling_TranslateDocumentAsync("Test.docx");
    }

    [TestMethod]
    public async Task Test_Pptx_Cancelling_TranslateDocumentAsync()
    {
        await Test_Cancelling_TranslateDocumentAsync("Test.pptx");
    }

    [TestMethod]
    public async Task Test_Xlsx_Cancelling_TranslateDocumentAsync()
    {
        await Test_Cancelling_TranslateDocumentAsync("Test.xlsx");
    }
}
