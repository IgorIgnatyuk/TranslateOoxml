namespace TranslateOoxmlServiceTests;

[TestClass]
public class TranslateOoxmlServiceTests
{
    private static readonly string testDir =
        "..\\..\\..\\..\\TranslateOoxmlIntegrationTests\\TestDocuments\\";

    private static readonly string inputDir = testDir + "Input\\";
    private static readonly string expectedOutputDir = testDir + "ExpectedOutput\\";

    private static bool StreamAreEqual(Stream stream1, Stream stream2)
    {
        int byte1, byte2;
        while ((byte1 = stream1.ReadByte()) == (byte2 = stream2.ReadByte()))
            if (byte1 == -1)
                return true;
        return false;
    }

    private static async Task PostToTranslateOoxmlService(
        Stream sourceStream,
        Stream targetStream,
        string targetLanguage,
        string serviceUrl)
    {
        var client = new HttpClient();

        using var requestHttpContent = new StreamContent(sourceStream);
        using var response =
            await client.PostAsync(serviceUrl + '/' + targetLanguage, requestHttpContent);

        using var responseHttpContent = response.Content;
        await responseHttpContent.CopyToAsync(targetStream);
    }

    private static void Test_PostToTranslateOoxmlService(string filename)
    {
        using var input = File.OpenRead(inputDir + filename);
        using var output = new MemoryStream();

        PostToTranslateOoxmlService(
            input,
            output,
            "DE",
            "https://localhost:7261/translate-ooxml")
            .Wait();

        output.Position = 0;
        using var expectedOutput = File.OpenRead(expectedOutputDir + filename);

        Assert.IsTrue(StreamAreEqual(output, expectedOutput));
    }

    [TestMethod]
    public void Test_TranslateDocument_Docx()
    {
        Test_PostToTranslateOoxmlService("Test.docx");
    }

    [TestMethod]
    public void Test_TranslateDocument_Pptx()
    {
        Test_PostToTranslateOoxmlService("Test.pptx");
    }

    [TestMethod]
    public void Test_TranslateDocument_Xlsx()
    {
        Test_PostToTranslateOoxmlService("Test.xlsx");
    }
}
