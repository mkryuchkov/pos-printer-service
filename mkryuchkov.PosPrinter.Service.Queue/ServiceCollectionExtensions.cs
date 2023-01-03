using Microsoft.Extensions.DependencyInjection;
using mkryuchkov.PosPrinter.Service.Core;
using mkryuchkov.PosPrinter.Service.Queue;

namespace mkryuchkov.TgBot.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQueueHandler<TEntity, TImplentation>(this IServiceCollection services)
            where TEntity : class
            where TImplentation : class, IQueueHandler<TEntity>
        {
            services.AddSingleton<IQueueHandler<TEntity>, TImplentation>();
            services.AddHostedService<QueueConsumer<TEntity>>();

            return services;
        }
    }
}