using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace mkryuchkov.PosPrinter.Service.Printing.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPrinterConfig(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<PrinterConfig>(configuration.GetSection(nameof(PrinterConfig)));

            return services;
        }
    }
}