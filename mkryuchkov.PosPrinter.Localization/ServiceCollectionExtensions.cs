using Microsoft.Extensions.DependencyInjection;

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
            return services;
        }
    }
}