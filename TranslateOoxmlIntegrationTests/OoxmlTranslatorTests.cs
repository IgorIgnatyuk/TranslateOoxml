using System.IO.Compression;
using static System.IO.File;
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

    private static void Test_TranslateZipArchiveMethod(
        string filename,
        Func<ZipArchive, Func<string, Task<string>>, Task<bool>> translateZipArchiveMethod)
    {
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        Copy(inputDir + filename, outputDir + filename, true);
        using (var zipArchive = ZipFile.Open(outputDir + filename, ZipArchiveMode.Update))
        {
            var translated = translateZipArchiveMethod(
                zipArchive,
                async (text) => await Translate(text, "DE"))
                .Result;

            Assert.IsTrue(translated);
        }
        Assert.IsTrue(FilesAreEqual(outputDir + filename, expectedOutputDir + filename));
    }

    [TestMethod]
    public void Test_TranslateDocxZipArchive()
    {
        Test_TranslateZipArchiveMethod("Test.docx", TranslateDocxZipArchive);
    }

    [TestMethod]
    public void Test_TranslatePptxZipArchive()
    {
        Test_TranslateZipArchiveMethod("Test.pptx", TranslatePptxZipArchive);
    }

    [TestMethod]
    public void Test_TranslateXlsxZipArchive()
    {
        Test_TranslateZipArchiveMethod("Test.xlsx", TranslateXlsxZipArchive);
    }

    private static void Test_TranslateZipArchive(string filename)
    {
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        Copy(inputDir + filename, outputDir + filename, true);
        using (var zipArchive = ZipFile.Open(outputDir + filename, ZipArchiveMode.Update))
        {
            TranslateZipArchive(
                zipArchive,
                async (text) => await Translate(text, "DE"))
                .Wait();

        }
        Assert.IsTrue(FilesAreEqual(outputDir + filename, expectedOutputDir + filename));
    }

    [TestMethod]
    public void Test_TranslateZipArchive_Docx()
    {
        Test_TranslateZipArchive("Test.docx");
    }

    [TestMethod]
    public void Test_TranslateZipArchive_Pptx()
    {
        Test_TranslateZipArchive("Test.pptx");
    }

    [TestMethod]
    public void Test_TranslateZipArchive_Xlsx()
    {
        Test_TranslateZipArchive("Test.xlsx");
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
