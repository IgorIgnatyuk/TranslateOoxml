using static TranslateOoxml.Test.Helpers.Equality;
using static TranslateOoxml.Test.TestDirectories;
using static TranslateOoxml.TranslateOoxmlServiceLib;

namespace TranslateOoxml.Test;

[TestClass]
public class TranslateOoxmlServiceLibTests
{
    private static async Task Test_ProcessPostTranslateOoxmlAsync(string filename)
    {
        using var input = File.OpenRead(inputDir + filename);
        using var output = new MemoryStream();

        await ProcessPostTranslateOoxmlAsync("DE", input, output, message => { });

        output.Position = 0;
        using var expectedOutput = File.OpenRead(expectedOutputDir + filename);
        Assert.IsTrue(StreamsAreEqual(output, expectedOutput));
    }

    [TestMethod]
    public async Task Test_Docx_ProcessPostTranslateOoxmlAsync()
    {
        await Test_ProcessPostTranslateOoxmlAsync("Test.docx");
    }

    [TestMethod]
    public async Task Test_Pptx_ProcessPostTranslateOoxmlAsync()
    {
        await Test_ProcessPostTranslateOoxmlAsync("Test.pptx");
    }

    [TestMethod]
    public async Task Test_Xlsx_ProcessPostTranslateOoxmlAsync()
    {
        await Test_ProcessPostTranslateOoxmlAsync("Test.xlsx");
    }
}
