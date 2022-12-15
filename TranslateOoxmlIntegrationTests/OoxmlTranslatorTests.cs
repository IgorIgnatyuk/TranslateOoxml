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

    private static void EnsureOutput()
    {
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);
    }

    private static void CopyToOutput(string filename)
    {
        EnsureOutput();
        Copy(inputDir + filename, outputDir + filename, true);
    }

    private static void AssertExpectedOutput(string filename)
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

    private static void Test_TranslateZipArchiveMethod(
        string filename,
        Func<ZipArchive, Func<string, Task<string>>, Task<bool>> translateZipArchiveMethod)
    {
        CopyToOutput(filename);
        using (var zipArchive = ZipFile.Open(outputDir + filename, ZipArchiveMode.Update))
        {
            var translated = translateZipArchiveMethod(
                zipArchive,
                async (text) => await TranslateXml(text, "DE"))
                .Result;

            Assert.IsTrue(translated);
        }
        AssertExpectedOutput(filename);
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

    private static void Test_TranslateZipArchiveMethod_WrongFormat(
        string filename,
        Func<ZipArchive, Func<string, Task<string>>, Task<bool>> translateZipArchiveMethod)
    {
        CopyToOutput(filename);
        using var zipArchive = ZipFile.Open(outputDir + filename, ZipArchiveMode.Update);

        var translated = translateZipArchiveMethod(
            zipArchive,
            async (text) => await TranslateXml(text, "DE"))
            .Result;

        Assert.IsFalse(translated);
    }

    [TestMethod]
    public void Test_TranslateDocxZipArchive_WrongFormat()
    {
        Test_TranslateZipArchiveMethod_WrongFormat("Test.pptx", TranslateDocxZipArchive);
    }

    [TestMethod]
    public void Test_TranslatePptxZipArchive_WrongFormat()
    {
        Test_TranslateZipArchiveMethod_WrongFormat("Test.xlsx", TranslatePptxZipArchive);
    }

    [TestMethod]
    public void Test_TranslateXlsxZipArchive_WrongFormat()
    {
        Test_TranslateZipArchiveMethod_WrongFormat("Test.docx", TranslateXlsxZipArchive);
    }

    private static void Test_TranslateZipArchive(string filename)
    {
        CopyToOutput(filename);
        using (var zipArchive = ZipFile.Open(outputDir + filename, ZipArchiveMode.Update))
        {
            TranslateZipArchive(
                zipArchive,
                async (text) => await TranslateXml(text, "DE"))
                .Wait();

        }
        AssertExpectedOutput(filename);
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

    private static void Test_TranslateZipArchive_WrongFormat(string filename)
    {
        CopyToOutput(filename);
        using var zipArchive = ZipFile.Open(outputDir + filename, ZipArchiveMode.Update);

        Assert.ThrowsExceptionAsync<Exception>(
            async () => await TranslateZipArchive(
                zipArchive,
                async (text) => await TranslateXml(text, "DE")));
    }

    [TestMethod]
    public void Test_TranslateZipArchive_WrongFormat_Zip()
    {
        Test_TranslateZipArchive_WrongFormat("Test.zip");
    }

    private static void Test_TranslateDocument(string filename)
    {
        EnsureOutput();

        TranslateDocument(
            inputDir + filename,
            outputDir + filename,
            async (text) => await TranslateXml(text, "DE"))
            .Wait();

        AssertExpectedOutput(filename);
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
                async (text) => await TranslateXml(text, "DE")));
    }

    [TestMethod]
    public void Test_TranslateDocument_FileNotFound_Txt()
    {
        Test_TranslateDocument_FileNotFound("Test.txt");
    }
}
