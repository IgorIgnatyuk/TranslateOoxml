using static TranslateOoxml.DeepLTranslator;
using static TranslateOoxml.OoxmlTranslator;

namespace TranslateOoxmlIntegrationTests;

[TestClass]
public class OoxmlTranslatorTests
{
    private static readonly string testDir = "..\\..\\..\\TestDocuments\\";
    private static readonly string inputDir = testDir + "Input\\";
    private static readonly string outputDir = testDir + "Output\\";
    private static readonly string expectedOutputDir = testDir + "ExpectedOutput\\";

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
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        TranslateDocument(
            inputDir + filename,
            outputDir + filename,
            async (text) => await Translate(text, "DE"))
            .Wait();

        Assert.IsTrue(FilesAreEqual(outputDir + filename, expectedOutputDir + filename));
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
}
