using static System.Environment;
using static TranslateOoxml.Constants;
using static TranslateOoxml.DeepLTranslator;

namespace TranslateOoxml.Test;

[TestClass]
public class DeepLTranslatorTests
{
    [TestMethod]
    public async Task Test_TranslateXml()
    {
        var s = await TranslateXml("That is a test", "DE");
        Assert.AreEqual(s, "Das ist ein Test");
    }

    [TestMethod]
    public async Task Test_TranslateXml_DEEPL_AUTH_KEY_unset()
    {
        var deepLAuthKey = GetEnvironmentVariable(DeepLAuthKey);
        SetEnvironmentVariable(DeepLAuthKey, null);
        try
        {
            await Assert.ThrowsExceptionAsync<Exception>(
                async () => { await TranslateXml("Test", "DE"); });
        }
        finally
        {
            SetEnvironmentVariable(DeepLAuthKey, deepLAuthKey);
        }
    }
}
