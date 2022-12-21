using System.Net;
using static Helpers.Equality;
using static TranslateOoxmlClient.TranslateOoxmlClientLib;

namespace TranslateOoxmlClientLibTests;

[TestClass]
public class TranslateOoxmlClientLibTests
{
    private static readonly string testDir =
        "..\\..\\..\\..\\TranslateOoxmlIntegrationTests\\TestDocuments\\";

    private static readonly string inputDir = testDir + "Input\\";
    private static readonly string outputDir = testDir + "Output\\";
    private static readonly string expectedOutputDir = testDir + "ExpectedOutput\\";

    private static HttpStatusCode Test_TranslateDocument(string filename)
    {
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        return TranslateDocument(
            inputDir + filename,
            outputDir + filename,
            "DE",
            "https://localhost:7261/translate-ooxml")
            .GetAwaiter().GetResult();
    }

    private static void Test_TranslateDocument(string filename, HttpStatusCode expectedStatusCode)
    {
        var statusCode = Test_TranslateDocument(filename);
        Assert.AreEqual(statusCode, expectedStatusCode);
        if (statusCode == HttpStatusCode.OK)
            Assert.IsTrue(FilesAreEqual(outputDir + filename, expectedOutputDir + filename));
    }

    [TestMethod]
    public void Test_TranslateDocument_Docx()
    {
        Test_TranslateDocument("Test.docx", HttpStatusCode.OK);
    }

    [TestMethod]
    public void Test_TranslateDocument_Pptx()
    {
        Test_TranslateDocument("Test.pptx", HttpStatusCode.OK);
    }

    [TestMethod]
    public void Test_TranslateDocument_Xlsx()
    {
        Test_TranslateDocument("Test.xlsx", HttpStatusCode.OK);
    }

    [TestMethod]
    public void Test_TranslateDocument_BadRequest_Zip()
    {
        Test_TranslateDocument("Test.zip", HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public void Test_TranslateDocument_BadRequest_Txt()
    {
        Test_TranslateDocument("Test.txt", HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public void Test_TranslateDocument_FileNotFound_Html()
    {
        Assert.ThrowsException<FileNotFoundException>(
            () => Test_TranslateDocument("Test.html"));
    }
}
