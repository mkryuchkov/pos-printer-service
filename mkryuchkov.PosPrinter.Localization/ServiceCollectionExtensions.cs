using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace mkryuchkov.PosPrinter.Localization
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedLocalization(this IServiceCollection services)
        {
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
            services.AddTransient<IStringLocalizer<Shared>, SharedStringLocalizer>();
            services.AddTransient<ISharedStringLocalizer, SharedStringLocalizer>();
            return services;
        }
    }
}