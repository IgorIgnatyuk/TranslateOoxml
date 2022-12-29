using System.IO.Compression;
using static System.IO.File;
using static TranslateOoxml.DeepLTranslator;
using static TranslateOoxml.Helpers.Equality;
using static TranslateOoxml.OoxmlTranslator;
using static TranslateOoxml.Test.TestDirectories;

namespace TranslateOoxml.Test;

[TestClass]
public class OoxmlTranslatorTests
{
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

    private static async Task Test_TranslateZipArchiveMethod(
        string filename,
        Func<ZipArchive, Func<string, Task<string>>, Task<bool>> translateZipArchiveMethod,
        bool expectedSuccess)
    {
        CopyToOutput(filename);
        bool success;
        using (var zipArchive = ZipFile.Open(outputDir + filename, ZipArchiveMode.Update))
        {
            success = await translateZipArchiveMethod(
                zipArchive,
                async (text) => await TranslateXml(text, "DE"));

            Assert.AreEqual(success, expectedSuccess);
        }
        if (success)
            AssertExpectedOutput(filename);
    }

    [TestMethod]
    public async Task Test_TranslateDocxZipArchive()
    {
        await Test_TranslateZipArchiveMethod("Test.docx", TranslateDocxZipArchive, true);
    }

    [TestMethod]
    public async Task Test_TranslatePptxZipArchive()
    {
        await Test_TranslateZipArchiveMethod("Test.pptx", TranslatePptxZipArchive, true);
    }

    [TestMethod]
    public async Task Test_TranslateXlsxZipArchive()
    {
        await Test_TranslateZipArchiveMethod("Test.xlsx", TranslateXlsxZipArchive, true);
    }

    [TestMethod]
    public async Task Test_TranslateDocxZipArchive_WrongFormat()
    {
        await Test_TranslateZipArchiveMethod("Test.pptx", TranslateDocxZipArchive, false);
    }

    [TestMethod]
    public async Task Test_TranslatePptxZipArchive_WrongFormat()
    {
        await Test_TranslateZipArchiveMethod("Test.xlsx", TranslatePptxZipArchive, false);
    }

    [TestMethod]
    public async Task Test_TranslateXlsxZipArchive_WrongFormat()
    {
        await Test_TranslateZipArchiveMethod("Test.docx", TranslateXlsxZipArchive, false);
    }

    private static async Task Test_TranslateZipArchive(string filename)
    {
        CopyToOutput(filename);
        using (var zipArchive = ZipFile.Open(outputDir + filename, ZipArchiveMode.Update))
        {
            await TranslateZipArchive(
                zipArchive,
                async (text) => await TranslateXml(text, "DE"));
        }
        AssertExpectedOutput(filename);
    }

    [TestMethod]
    public async Task Test_TranslateZipArchive_Docx()
    {
        await Test_TranslateZipArchive("Test.docx");
    }

    [TestMethod]
    public async Task Test_TranslateZipArchive_Pptx()
    {
        await Test_TranslateZipArchive("Test.pptx");
    }

    [TestMethod]
    public async Task Test_TranslateZipArchive_Xlsx()
    {
        await Test_TranslateZipArchive("Test.xlsx");
    }

    [TestMethod]
    public async Task Test_TranslateZipArchive_WrongFormat_Zip()
    {
        await Assert.ThrowsExceptionAsync<UnsupportedFileFormatException>(
            async () => await Test_TranslateZipArchive("Test.zip"));
    }

    private static async Task Test_TranslateDocument(string filename)
    {
        EnsureOutput();

        await TranslateDocument(
            inputDir + filename,
            outputDir + filename,
            async (text) => await TranslateXml(text, "DE"));

        AssertExpectedOutput(filename);
    }

    [TestMethod]
    public async Task Test_TranslateDocument_Docx()
    {
        await Test_TranslateDocument("Test.docx");
    }

    [TestMethod]
    public async Task Test_TranslateDocument_Pptx()
    {
        await Test_TranslateDocument("Test.pptx");
    }

    [TestMethod]
    public async Task Test_TranslateDocument_Xlsx()
    {
        await Test_TranslateDocument("Test.xlsx");
    }

    [TestMethod]
    public async Task Test_TranslateDocument_FileNotFound_Html()
    {
        await Assert.ThrowsExceptionAsync<FileNotFoundException>(
            async () => await Test_TranslateDocument("Test.html"));
    }

    [TestMethod]
    public async Task Test_TranslateDocument_UnsupportedFileFormat_Zip()
    {
        await Assert.ThrowsExceptionAsync<UnsupportedFileFormatException>(
            async () => await Test_TranslateDocument("Test.zip"));
    }

    [TestMethod]
    public async Task Test_TranslateDocument_UnsupportedFileFormat_Txt()
    {
        await Assert.ThrowsExceptionAsync<UnsupportedFileFormatException>(
            async () => await Test_TranslateDocument("Test.txt"));
    }
}
