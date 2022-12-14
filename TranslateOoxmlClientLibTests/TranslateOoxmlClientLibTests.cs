using System.Net;
using static TranslateOoxml.Test.Helpers.Equality;
using static TranslateOoxml.Test.TestDirectories;
using static TranslateOoxml.TranslateOoxmlClientLib;

namespace TranslateOoxml.Test;

[TestClass]
public class TranslateOoxmlClientLibTests
{
    private static async Task<HttpStatusCode> Test_TranslateDocument(string filename)
    {
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        return await TranslateDocument(
            inputDir + filename,
            outputDir + filename,
            "DE",
            "https://localhost:7261/translate-ooxml");
    }

    private static async Task Test_TranslateDocument(
        string filename,
        HttpStatusCode expectedStatusCode)
    {
        var statusCode = await Test_TranslateDocument(filename);
        Assert.AreEqual(statusCode, expectedStatusCode);
        if (statusCode == HttpStatusCode.OK)
            Assert.IsTrue(FilesAreEqual(outputDir + filename, expectedOutputDir + filename));
    }

    [TestMethod]
    public async Task Test_TranslateDocument_Docx()
    {
        await Test_TranslateDocument("Test.docx", HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task Test_TranslateDocument_Pptx()
    {
        await Test_TranslateDocument("Test.pptx", HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task Test_TranslateDocument_Xlsx()
    {
        await Test_TranslateDocument("Test.xlsx", HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task Test_TranslateDocument_BadRequest_Zip()
    {
        await Test_TranslateDocument("Test.zip", HttpStatusCode.UnsupportedMediaType);
    }

    [TestMethod]
    public async Task Test_TranslateDocument_BadRequest_Txt()
    {
        await Test_TranslateDocument("Test.txt", HttpStatusCode.UnsupportedMediaType);
    }

    [TestMethod]
    public async Task Test_TranslateDocument_FileNotFound_Html()
    {
        await Assert.ThrowsExceptionAsync<FileNotFoundException>(
            async () => await Test_TranslateDocument("Test.html"));
    }
}
