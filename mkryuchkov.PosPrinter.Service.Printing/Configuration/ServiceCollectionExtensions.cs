using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using mkryuchkov.PosPrinter.Service.Printing.Interfaces;

namespace mkryuchkov.PosPrinter.Service.Printing.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQueryPrinting(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<PrinterConfig>(configuration.GetSection(nameof(PrinterConfig)));
            services.AddSingleton<IPrintQueryTranslator, PrintQueryTranslator>();

            return services;
        }
    }
}