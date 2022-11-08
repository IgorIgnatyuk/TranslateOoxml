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
    }
}
