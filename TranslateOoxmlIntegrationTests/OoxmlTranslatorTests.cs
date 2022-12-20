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
        int b;
        while ((b = stream1.ReadByte()) == stream2.ReadByte())
            if (b == -1)
                return true;
        return false;
    }

    private static void Test_TranslateZipArchiveMethod(
        string filename,
        Func<ZipArchive, Func<string, Task<string>>, Task<bool>> translateZipArchiveMethod,
        bool expectedSuccess)
    {
        CopyToOutput(filename);
        bool success;
        using (var zipArchive = ZipFile.Open(outputDir + filename, ZipArchiveMode.Update))
        {
            success = translateZipArchiveMethod(
                zipArchive,
                async (text) => await TranslateXml(text, "DE"))
                .Result;

            Assert.AreEqual(success, expectedSuccess);
        }
        if (success)
            AssertExpectedOutput(filename);
    }

    [TestMethod]
    public void Test_TranslateDocxZipArchive()
    {
        Test_TranslateZipArchiveMethod("Test.docx", TranslateDocxZipArchive, true);
    }

    [TestMethod]
    public void Test_TranslatePptxZipArchive()
    {
        Test_TranslateZipArchiveMethod("Test.pptx", TranslatePptxZipArchive, true);
    }

    [TestMethod]
    public void Test_TranslateXlsxZipArchive()
    {
        Test_TranslateZipArchiveMethod("Test.xlsx", TranslateXlsxZipArchive, true);
    }

    [TestMethod]
    public void Test_TranslateDocxZipArchive_WrongFormat()
    {
        Test_TranslateZipArchiveMethod("Test.pptx", TranslateDocxZipArchive, false);
    }

    [TestMethod]
    public void Test_TranslatePptxZipArchive_WrongFormat()
    {
        Test_TranslateZipArchiveMethod("Test.xlsx", TranslatePptxZipArchive, false);
    }

    [TestMethod]
    public void Test_TranslateXlsxZipArchive_WrongFormat()
    {
        Test_TranslateZipArchiveMethod("Test.docx", TranslateXlsxZipArchive, false);
    }

    private static void Test_TranslateZipArchive(string filename)
    {
        CopyToOutput(filename);
        using (var zipArchive = ZipFile.Open(outputDir + filename, ZipArchiveMode.Update))
        {
            TranslateZipArchive(
                zipArchive,
                async (text) => await TranslateXml(text, "DE"))
                .GetAwaiter().GetResult();
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

    [TestMethod]
    public void Test_TranslateZipArchive_WrongFormat_Zip()
    {
        Assert.ThrowsException<UnsupportedFileFormatException>(
            () => Test_TranslateZipArchive("Test.zip"));
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

        Assert.ThrowsException<FileNotFoundException>(
            () => TranslateDocument(
                inputDir + filename,
                outputDir + filename,
                async (text) => await TranslateXml(text, "DE"))
            .GetAwaiter().GetResult());
    }

    [TestMethod]
    public void Test_TranslateDocument_FileNotFound_Html()
    {
        Test_TranslateDocument_FileNotFound("Test.html");
    }

    private static void Test_TranslateDocument_InvalidDataException(string filename)
    {
        EnsureOutput();

        Assert.ThrowsException<InvalidDataException>(
            () => TranslateDocument(
                inputDir + filename,
                outputDir + filename,
                async (text) => await TranslateXml(text, "DE"))
            .GetAwaiter().GetResult());
    }

    [TestMethod]
    public void Test_TranslateDocument_InvalidDataException_Txt()
    {
        Test_TranslateDocument_InvalidDataException("Test.txt");
    }
}
