using Microsoft.Extensions.Localization;

namespace mkryuchkov.PosPrinter.Localization
{
    public static class LocalizedStringExtensions
    {
        public static string ReEscape(this string source)
        {
            return source.Replace("\\n", "\n");
        }

        public static string ReEscape(this LocalizedString source)
        {
            return source.ToString().ReEscape();
        }
    }
}