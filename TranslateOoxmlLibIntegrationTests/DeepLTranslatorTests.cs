using static System.Environment;
using static TranslateOoxml.Constants;
using static TranslateOoxml.DeepLTranslator;

namespace TranslateOoxml.Test;

[TestClass]
public class DeepLTranslatorTests
{
    [TestMethod]
    public async Task Test_TranslateXmlAsync()
    {
        var s = await TranslateXmlAsync("That is a test", "DE");
        Assert.AreEqual(s, "Das ist ein Test");
    }

    [TestMethod]
    public async Task Test_DEEPL_AUTH_KEY_unset_TranslateXmlAsync()
    {
        var deepLAuthKey = GetEnvironmentVariable(DeepLAuthKey);
        SetEnvironmentVariable(DeepLAuthKey, null);
        try
        {
            await Assert.ThrowsExceptionAsync<DeepLTranslatorException>(
                async () => { await TranslateXmlAsync("Test", "DE"); });
        }
        finally
        {
            SetEnvironmentVariable(DeepLAuthKey, deepLAuthKey);
        }
    }

    [TestMethod]
    public async Task Test_Cancelling_TranslateXmlAsync()
    {
        using var cts = new CancellationTokenSource();
        var task = TranslateXmlAsync("Test", "DE", cts.Token);
        await Task.Delay(100);
        cts.Cancel();
        await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () => await task);
    }
}
