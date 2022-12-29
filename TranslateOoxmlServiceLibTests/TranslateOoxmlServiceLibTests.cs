using static TranslateOoxml.Test.Helpers.Equality;
using static TranslateOoxml.Test.TestDirectories;
using static TranslateOoxml.TranslateOoxmlServiceLib;

namespace TranslateOoxml.Test;

[TestClass]
public class TranslateOoxmlServiceLibTests
{
    private static async Task Test_ProcessPostTranslateOoxml(string filename)
    {
        using var input = File.OpenRead(inputDir + filename);
        using var output = new MemoryStream();

        await ProcessPostTranslateOoxml("DE", input, output, message => { });

        output.Position = 0;
        using var expectedOutput = File.OpenRead(expectedOutputDir + filename);
        Assert.IsTrue(StreamsAreEqual(output, expectedOutput));
    }

    [TestMethod]
    public async Task Test_TranslateDocument_Docx()
    {
        await Test_ProcessPostTranslateOoxml("Test.docx");
    }

    [TestMethod]
    public async Task Test_TranslateDocument_Pptx()
    {
        await Test_ProcessPostTranslateOoxml("Test.pptx");
    }

    [TestMethod]
    public async Task Test_TranslateDocument_Xlsx()
    {
        await Test_ProcessPostTranslateOoxml("Test.xlsx");
    }
}
