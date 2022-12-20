using System.Net;
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

    private static bool FilesAreEqual(string path1, string path2)
    {
        using var stream1 = File.OpenRead(path1);
        using var stream2 = File.OpenRead(path2);
        int b;
        while ((b = stream1.ReadByte()) == stream2.ReadByte())
            if (b == -1)
                return true;
        return false;
    }

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
    public void Test_TranslateDocument_Zip()
    {
        Test_TranslateDocument("Test.zip", HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public void Test_TranslateDocument_Txt()
    {
        Test_TranslateDocument("Test.txt", HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public void Test_TranslateDocument_Html()
    {
        Assert.ThrowsException<FileNotFoundException>(() => Test_TranslateDocument("Test.html"));
    }
}
