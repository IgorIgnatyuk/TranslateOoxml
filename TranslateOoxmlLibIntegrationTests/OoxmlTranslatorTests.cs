using System.IO.Compression;
using static System.IO.File;
using static TranslateOoxml.DeepLTranslator;
using static TranslateOoxml.OoxmlTranslator;
using static TranslateOoxml.Test.Helpers.Equality;
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

    private static void DeleteFromOutput(string filename)
    {
        Delete(outputDir + filename);
    }

    private static async Task Test_TranslateZipArchiveMethodAsync(
        string filename,
        Func<
            ZipArchive,
            Func<string, CancellationToken, Task<string>>,
            CancellationToken,
            Task<bool>
            > translateZipArchiveMethodAsync,
        bool expectedSuccess)
    {
        try
        {
            CopyToOutput(filename);
            bool success;
            using (var zipArchive = ZipFile.Open(outputDir + filename, ZipArchiveMode.Update))
            {
                success = await translateZipArchiveMethodAsync(
                    zipArchive,
                    async (text, cancellationToken) => await TranslateXmlAsync(
                        text,
                        "DE",
                        cancellationToken),
                   default);

                Assert.AreEqual(success, expectedSuccess);
            }
            if (success)
                AssertExpectedOutput(filename);
        }
        finally
        {
            DeleteFromOutput(filename);
        }
    }

    [TestMethod]
    public async Task Test_TranslateDocxZipArchiveAsync()
    {
        await Test_TranslateZipArchiveMethodAsync("Test.docx", TranslateDocxZipArchiveAsync, true);
    }

    [TestMethod]
    public async Task Test_TranslatePptxZipArchiveAsync()
    {
        await Test_TranslateZipArchiveMethodAsync("Test.pptx", TranslatePptxZipArchiveAsync, true);
    }

    [TestMethod]
    public async Task Test_TranslateXlsxZipArchiveAsync()
    {
        await Test_TranslateZipArchiveMethodAsync("Test.xlsx", TranslateXlsxZipArchiveAsync, true);
    }

    [TestMethod]
    public async Task Test_WrongFormat_TranslateDocxZipArchiveAsync()
    {
        await Test_TranslateZipArchiveMethodAsync("Test.pptx", TranslateDocxZipArchiveAsync, false);
    }

    [TestMethod]
    public async Task Test_WrongFormat_TranslatePptxZipArchiveAsync()
    {
        await Test_TranslateZipArchiveMethodAsync("Test.xlsx", TranslatePptxZipArchiveAsync, false);
    }

    [TestMethod]
    public async Task Test_WrongFormat_TranslateXlsxZipArchiveAsync()
    {
        await Test_TranslateZipArchiveMethodAsync("Test.docx", TranslateXlsxZipArchiveAsync, false);
    }

    private static async Task Test_Cancelling_TranslateZipArchiveMethodAsync(
        string filename,
        Func<
            ZipArchive,
            Func<string, CancellationToken, Task<string>>,
            CancellationToken,
            Task<bool>
            > translateZipArchiveMethodAsync)
    {
        try
        {
            CopyToOutput(filename);
            using (var zipArchive = ZipFile.Open(outputDir + filename, ZipArchiveMode.Update))
            {
                using var cts = new CancellationTokenSource();
                var task = translateZipArchiveMethodAsync(
                    zipArchive,
                    async (text, cancellationToken) => await TranslateXmlAsync(
                        text,
                        "DE",
                        cancellationToken),
                   cts.Token);

                await Task.Delay(1);
                cts.Cancel();
                await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () => await task);
            }
        }
        finally
        {
            DeleteFromOutput(filename);
        }
    }

    [TestMethod]
    public async Task Test_Cancelling_TranslateDocxZipArchiveAsync()
    {
        await Test_Cancelling_TranslateZipArchiveMethodAsync(
            "Test.docx",
            TranslateDocxZipArchiveAsync);
    }

    [TestMethod]
    public async Task Test_Cancelling_TranslatePptxZipArchiveAsync()
    {
        await Test_Cancelling_TranslateZipArchiveMethodAsync(
            "Test.pptx",
            TranslatePptxZipArchiveAsync);
    }

    [TestMethod]
    public async Task Test_Cancelling_TranslateXlsxZipArchiveAsync()
    {
        await Test_Cancelling_TranslateZipArchiveMethodAsync(
            "Test.xlsx",
            TranslateXlsxZipArchiveAsync);
    }

    private static async Task Test_TranslateZipArchiveAsync(string filename)
    {
        try
        {
            CopyToOutput(filename);
            using (var zipArchive = ZipFile.Open(outputDir + filename, ZipArchiveMode.Update))
            {
                await TranslateZipArchiveAsync(
                    zipArchive,
                    async (text, cancellationToken) => await TranslateXmlAsync(
                        text,
                        "DE",
                        cancellationToken));
            }
            AssertExpectedOutput(filename);
        }
        finally
        {
            DeleteFromOutput(filename);
        }
    }

    [TestMethod]
    public async Task Test_Docx_TranslateZipArchiveAsync()
    {
        await Test_TranslateZipArchiveAsync("Test.docx");
    }

    [TestMethod]
    public async Task Test_Pptx_TranslateZipArchiveAsync()
    {
        await Test_TranslateZipArchiveAsync("Test.pptx");
    }

    [TestMethod]
    public async Task Test_Xlsx_TranslateZipArchiveAsync()
    {
        await Test_TranslateZipArchiveAsync("Test.xlsx");
    }

    [TestMethod]
    public async Task Test_WrongFormat_Zip_TranslateZipArchiveAsync()
    {
        await Assert.ThrowsExceptionAsync<UnsupportedFileFormatException>(
            async () => await Test_TranslateZipArchiveAsync("Test.zip"));
    }

    private static async Task Test_Cancelling_TranslateZipArchiveAsync(string filename)
    {
        try
        {
            CopyToOutput(filename);
            using (var zipArchive = ZipFile.Open(outputDir + filename, ZipArchiveMode.Update))
            {
                using var cts = new CancellationTokenSource();
                var task = TranslateZipArchiveAsync(
                    zipArchive,
                    async (text, cancellationToken) => await TranslateXmlAsync(
                        text,
                        "DE",
                        cancellationToken),
                    cts.Token);

                await Task.Delay(1);
                cts.Cancel();
                await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () => await task);
            }
        }
        finally
        {
            DeleteFromOutput(filename);
        }
    }

    [TestMethod]
    public async Task Test_Docx_Cancelling_TranslateZipArchiveAsync()
    {
        await Test_Cancelling_TranslateZipArchiveAsync("Test.docx");
    }

    [TestMethod]
    public async Task Test_Pptx_Cancelling_TranslateZipArchiveAsync()
    {
        await Test_Cancelling_TranslateZipArchiveAsync("Test.pptx");
    }

    [TestMethod]
    public async Task Test_Xlsx_Cancelling_TranslateZipArchiveAsync()
    {
        await Test_Cancelling_TranslateZipArchiveAsync("Test.xlsx");
    }

    private static async Task Test_TranslateDocumentAsync(string filename)
    {
        try
        {
            EnsureOutput();

            await TranslateDocumentAsync(
                inputDir + filename,
                outputDir + filename,
                async (text, cancellationToken) => await TranslateXmlAsync(
                    text,
                    "DE",
                    cancellationToken));

            AssertExpectedOutput(filename);
        }
        finally
        {
            DeleteFromOutput(filename);
        }
    }

    [TestMethod]
    public async Task Test_Docx_TranslateDocumentAsync()
    {
        await Test_TranslateDocumentAsync("Test.docx");
    }

    [TestMethod]
    public async Task Test_Pptx_TranslateDocumentAsync()
    {
        await Test_TranslateDocumentAsync("Test.pptx");
    }

    [TestMethod]
    public async Task Test_Xlsx_TranslateDocumentAsync()
    {
        await Test_TranslateDocumentAsync("Test.xlsx");
    }

    [TestMethod]
    public async Task Test_FileNotFound_Html_TranslateDocumentAsync()
    {
        await Assert.ThrowsExceptionAsync<FileNotFoundException>(
            async () => await Test_TranslateDocumentAsync("Test.html"));
    }

    [TestMethod]
    public async Task Test_UnsupportedFileFormat_Zip_TranslateDocumentAsync()
    {
        await Assert.ThrowsExceptionAsync<UnsupportedFileFormatException>(
            async () => await Test_TranslateDocumentAsync("Test.zip"));
    }

    [TestMethod]
    public async Task Test_UnsupportedFileFormat_Txt_TranslateDocumentAsync()
    {
        await Assert.ThrowsExceptionAsync<UnsupportedFileFormatException>(
            async () => await Test_TranslateDocumentAsync("Test.txt"));
    }

    private static async Task Test_Cancelling_TranslateDocumentAsync(string filename)
    {
        try
        {
            EnsureOutput();

            using var cts = new CancellationTokenSource();
            var task = TranslateDocumentAsync(
                inputDir + filename,
                outputDir + filename,
                async (text, cancellationToken) => await TranslateXmlAsync(
                    text,
                    "DE",
                    cancellationToken),
                cts.Token);

            await Task.Delay(1);
            cts.Cancel();
            await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () => await task);
        }
        finally
        {
            DeleteFromOutput(filename);
        }
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
