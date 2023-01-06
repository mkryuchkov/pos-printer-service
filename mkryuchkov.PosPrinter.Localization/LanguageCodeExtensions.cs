using System.Globalization;

namespace mkryuchkov.PosPrinter.Localization
{
    public static class LanguageCodeExtensions
    {
        public static void SetCurrentUICulture(this string? languageCode)
        {
            if (!string.IsNullOrWhiteSpace(languageCode))
            {
                var culture = languageCode switch
                {
                    "ru" => new CultureInfo("ru-ru"),
                    _ => new CultureInfo("en-us")
                };
                CultureInfo.CurrentUICulture = culture;
                CultureInfo.CurrentCulture = culture;
            }
        }
    }
}