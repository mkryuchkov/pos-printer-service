using Microsoft.Extensions.Localization;

namespace mkryuchkov.PosPrinter.Localization
{
    public interface ISharedStringLocalizer : IStringLocalizer<Shared>
    {
        void SetCurrentCultures(string? langCode);

        LocalizedString this[string? langCode, string name] { get; }

        LocalizedString this[string? langCode, string name, params object[] arguments] { get; }
    }
}