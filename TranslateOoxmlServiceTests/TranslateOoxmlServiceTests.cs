using System.Net;
using static Helpers.Equality;

namespace TranslateOoxmlServiceTests;

[TestClass]
public class TranslateOoxmlServiceTests
{
    private static readonly string testDir = "..\\..\\..\\..\\TestDocuments\\";
    private static readonly string inputDir = testDir + "Input\\";
    private static readonly string expectedOutputDir = testDir + "ExpectedOutput\\";

    private static readonly HttpClient HttpClient;

    static TranslateOoxmlServiceTests()
    {
        HttpClient = new HttpClient();
    }

    private static async Task<HttpStatusCode> PostToTranslateOoxmlService(
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

    private static void Test_PostToTranslateOoxmlService(
        string filename,
        HttpStatusCode expectedStatusCode)
    {
        using var input = File.OpenRead(inputDir + filename);
        using var output = new MemoryStream();

        var statusCode = PostToTranslateOoxmlService(
            input,
            output,
            "DE",
            "https://localhost:7261/translate-ooxml")
            .Result;

        Assert.AreEqual(statusCode, expectedStatusCode);

        if (statusCode == HttpStatusCode.OK)
        {
            output.Position = 0;
            using var expectedOutput = File.OpenRead(expectedOutputDir + filename);
            Assert.IsTrue(StreamsAreEqual(output, expectedOutput));
        }
    }

    [TestMethod]
    public void Test_TranslateDocument_Docx()
    {
        Test_PostToTranslateOoxmlService("Test.docx", HttpStatusCode.OK);
    }

    [TestMethod]
    public void Test_TranslateDocument_Pptx()
    {
        Test_PostToTranslateOoxmlService("Test.pptx", HttpStatusCode.OK);
    }

    [TestMethod]
    public void Test_TranslateDocument_Xlsx()
    {
        Test_PostToTranslateOoxmlService("Test.xlsx", HttpStatusCode.OK);
    }

    [TestMethod]
    public void Test_TranslateDocument_BadRequest_Zip()
    {
        Test_PostToTranslateOoxmlService("Test.zip", HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public void Test_TranslateDocument_BadRequest_Txt()
    {
        Test_PostToTranslateOoxmlService("Test.txt", HttpStatusCode.BadRequest);
    }
}
