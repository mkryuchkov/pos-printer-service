using Microsoft.Extensions.DependencyInjection;
using mkryuchkov.PosPrinter.Service.Core;
using mkryuchkov.PosPrinter.Service.Queue;

namespace mkryuchkov.TgBot.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQueueHandler<TEntity, TImplentation>(this IServiceCollection services)
            where TImplentation : IQueueHandler<TEntity>
        {
            services.AddScoped<IQueueHandler<TEntity>>();
            services.AddHostedService<QueueConsumer<TEntity>>();

            return services;
        }
    }
}