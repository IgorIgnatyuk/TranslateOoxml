namespace TranslateOoxml;

class TranslateResult
{
    public class Translation
    {
        public Translation(string detected_source_language, string text)
        {
            Detected_source_language = detected_source_language;
            Text = text;
        }

        public string Detected_source_language { get; }
        public string Text { get; }
    }

    public TranslateResult(Translation[] translations)
    {
        this.Translations = translations;
    }

    public Translation[] Translations { get; }
}
