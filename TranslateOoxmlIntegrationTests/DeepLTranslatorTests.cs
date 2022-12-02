using static System.Environment;
using static TranslateOoxml.Constants;
using static TranslateOoxml.DeepLTranslator;

namespace TranslateOoxmlIntegrationTests;

[TestClass]
public class DeepLTranslatorTests
{
    [TestMethod]
    public void Test_TranslateXml()
    {
        var s = TranslateXml("That is a test", "DE").Result;
        Assert.AreEqual(s, "Das ist ein Test");
    }

    [TestMethod]
    public void Test_TranslateXml_DEEPL_AUTH_KEY_unset()
    {
        var deepLAuthKey = GetEnvironmentVariable(DeepLAuthKey);
        SetEnvironmentVariable(DeepLAuthKey, null);
        try
        {
            Assert.ThrowsExceptionAsync<Exception>(
                async () => { await TranslateXml("Test", "DE"); });
        }
        finally
        {
            SetEnvironmentVariable(DeepLAuthKey, deepLAuthKey);
        }
    }
}
