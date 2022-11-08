using static System.IO.File;
using static TranslateOoxml.DeepLTranslator;

namespace TranslateOoxmlIntegrationTests
{
    [TestClass]
    public class DeepLTranslatorTests
    {
        [TestMethod]
        public void Test_Translate()
        {
            var s = Translate("That is a test", "DE").Result;
            Assert.AreEqual(s, "Das ist ein Test");
        }

        private static readonly string testDir = "..\\..\\..\\TestDocuments\\";
        private static readonly string inputFile = testDir + "Input\\Test";
        private static readonly string outputDir = testDir + "Output\\";
        private static readonly string outputFile = outputDir + "Test";
        private static readonly string expectedOutputFile = testDir + "ExpectedOutput\\Test";

        private static readonly Func<string, Task<string>> translate =
            async (text) => await Translate(text, "DE");

        private static bool FilesAreEqual(string path1, string path2)
        {
            using var stream1 = File.OpenRead(path1);
            using var stream2 = File.OpenRead(path2);
            int byte1, byte2;
            while ((byte1 = stream1.ReadByte()) == (byte2 = stream2.ReadByte()) && byte1 != -1)
                ;
            return byte1 == -1 && byte2 == -
                1;
        }

        private static void EnsureExistingOutputDirectory()
        {
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);
        }

        [TestMethod]
        public void Test_TranslateDocument_Docx()
        {
            EnsureExistingOutputDirectory();
            TranslateDocument(inputFile + ".docx", outputFile + ".docx", translate).Wait();
            Assert.IsTrue(FilesAreEqual(outputFile + ".docx", expectedOutputFile + ".docx"));
        }

        [TestMethod]
        public void Test_TranslateDocument_Pptx()
        {
            EnsureExistingOutputDirectory();
            TranslateDocument(inputFile + ".pptx", outputFile + ".pptx", translate).Wait();
            Assert.IsTrue(FilesAreEqual(outputFile + ".pptx", expectedOutputFile + ".pptx"));
        }

        [TestMethod]
        public void Test_TranslateDocument_Xlsx()
        {
            EnsureExistingOutputDirectory();
            TranslateDocument(inputFile + ".xlsx", outputFile + ".xlsx", translate).Wait();
            Assert.IsTrue(FilesAreEqual(outputFile + ".xlsx", expectedOutputFile + ".xlsx"));
        }
    }
}
