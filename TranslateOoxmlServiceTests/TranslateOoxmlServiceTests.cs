using System.Net;
using static TranslateOoxml.Test.Helpers.Equality;
using static TranslateOoxml.Test.TestDirectories;

namespace TranslateOoxml.Test;

[TestClass]
public class TranslateOoxmlServiceTests
{
    private static readonly HttpClient HttpClient;

    static TranslateOoxmlServiceTests()
    {
        HttpClient = new HttpClient();
    }

    private static async Task<HttpStatusCode> PostToTranslateOoxmlServiceAsync(
        Stream sourceStream,
        Stream targetStream,
        string targetLanguage,
        string serviceUrl)
    {
        using var requestHttpContent = new StreamContent(sourceStream);
        using var response =
            await HttpClient.PostAsync(serviceUrl + '/' + targetLanguage, requestHttpContent);

        using var responseHttpContent = response.Content;
        await responseHttpContent.CopyToAsync(targetStream);
        return response.StatusCode;
    }

    private static async Task Test_PostToTranslateOoxmlServiceAsync(
        string filename,
        HttpStatusCode expectedStatusCode)
    {
        using var input = File.OpenRead(inputDir + filename);
        using var output = new MemoryStream();

        var statusCode = await PostToTranslateOoxmlServiceAsync(
            input,
            output,
            "DE",
            "https://localhost:7261/translate-ooxml");

        Assert.AreEqual(statusCode, expectedStatusCode);

        if (statusCode == HttpStatusCode.OK)
        {
            output.Position = 0;
            using var expectedOutput = File.OpenRead(expectedOutputDir + filename);
            Assert.IsTrue(StreamsAreEqual(output, expectedOutput));
        }
    }

    [TestMethod]
    public async Task Test_Docx_PostToTranslateOoxmlServiceAsync()
    {
        await Test_PostToTranslateOoxmlServiceAsync(
            "Test.docx",
            HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task Test_Pptx_PostToTranslateOoxmlServiceAsync()
    {
        await Test_PostToTranslateOoxmlServiceAsync(
            "Test.pptx",
            HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task Test_Xlsx_PostToTranslateOoxmlServiceAsync()
    {
        await Test_PostToTranslateOoxmlServiceAsync(
            "Test.xlsx",
            HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task Test_BadRequest_Zip_PostToTranslateOoxmlServiceAsync()
    {
        await Test_PostToTranslateOoxmlServiceAsync(
            "Test.zip",
            HttpStatusCode.UnsupportedMediaType);
    }

    [TestMethod]
    public async Task Test_BadRequest_Txt_PostToTranslateOoxmlServiceAsync()
    {
        await Test_PostToTranslateOoxmlServiceAsync(
            "Test.txt",
            HttpStatusCode.UnsupportedMediaType);
    }
}
