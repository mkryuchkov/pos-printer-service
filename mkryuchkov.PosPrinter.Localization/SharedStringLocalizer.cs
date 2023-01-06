using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace mkryuchkov.PosPrinter.Localization
{
    public class SharedStringLocalizer : ISharedStringLocalizer
    {
        private ILogger<SharedStringLocalizer> _logger;
        private IStringLocalizer _localizer;

        public SharedStringLocalizer(
            ILogger<SharedStringLocalizer> logger,
            IStringLocalizerFactory factory)
        {
            _logger = logger;
            _localizer = factory.Create(typeof(Shared));
        }

        public LocalizedString this[string name]
        {
            get
            {
                var result = _localizer[name];
                return new LocalizedString(
                    result.Name,
                    ReEscape(result.Value),
                    result.ResourceNotFound,
                    result.SearchedLocation);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var result = _localizer[name, arguments];
                return new LocalizedString(
                    result.Name,
                    ReEscape(result.Value),
                    result.ResourceNotFound,
                    result.SearchedLocation);
            }
        }

        public void SetCurrentCultures(string? langCode)
        {
            if (!string.IsNullOrWhiteSpace(langCode))
            {
                var cultureInfo = langCode switch
                {
                    "ru" => new CultureInfo("ru-ru"),
                    _ => new CultureInfo("en-us")
                };
                CultureInfo.CurrentUICulture = cultureInfo;
                CultureInfo.CurrentCulture = cultureInfo;
            }
        }

        public LocalizedString this[string? langCode, string name]
        {
            get
            {
                SetCurrentCultures(langCode);
                return this[name];
            }
        }

        public LocalizedString this[string? langCode, string name, params object[] arguments]
        {
            get
            {
                SetCurrentCultures(langCode);
                return this[name, arguments];
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _localizer.GetAllStrings(includeParentCultures);
        }

        private string ReEscape(string source) => source.Replace("\\n", "\n");
    }
}