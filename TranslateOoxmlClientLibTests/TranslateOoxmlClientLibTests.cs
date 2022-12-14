using static System.IO.File;
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

    private static void EnsureOutput()
    {
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);
    }

    private static void AssertEqualInputOutput(string filename)
    {
        Assert.IsTrue(FilesAreEqual(outputDir + filename, expectedOutputDir + filename));
    }

    private static bool FilesAreEqual(string path1, string path2)
    {
        using var stream1 = File.OpenRead(path1);
        using var stream2 = File.OpenRead(path2);
        int byte1, byte2;
        while ((byte1 = stream1.ReadByte()) == (byte2 = stream2.ReadByte()))
            if (byte1 == -1)
                return true;
        return false;
    }
    private static void Test_TranslateDocument(string filename)
    {
        EnsureOutput();

        TranslateDocument(
            inputDir + filename,
            outputDir + filename,
            "DE",
            "https://localhost:7261/translate-ooxml")
            .Wait();

        AssertEqualInputOutput(filename);
    }

    [TestMethod]
    public void Test_TranslateDocument_Docx()
    {
        Test_TranslateDocument("Test.docx");
    }

    [TestMethod]
    public void Test_TranslateDocument_Pptx()
    {
        Test_TranslateDocument("Test.pptx");
    }

    [TestMethod]
    public void Test_TranslateDocument_Xlsx()
    {
        Test_TranslateDocument("Test.xlsx");
    }

    private static void Test_TranslateDocument_FileNotFound(string filename)
    {
        EnsureOutput();

        Assert.ThrowsExceptionAsync<FileNotFoundException>(
            async () => await TranslateDocument(
                inputDir + filename,
                outputDir + filename,
                "DE",
                "https://localhost:7261/translate-ooxml"));
    }

    [TestMethod]
    public void Test_TranslateDocument_FileNotFound_Txt()
    {
        Test_TranslateDocument_FileNotFound("Test.txt");
    }
}
