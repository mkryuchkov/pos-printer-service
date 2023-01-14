using Microsoft.Extensions.DependencyInjection;
using mkryuchkov.PosPrinter.Service.Core;

namespace mkryuchkov.PosPrinter.Service.Queue
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQueueHandler<TEntity, TImplementation>(this IServiceCollection services)
            where TEntity : class
            where TImplementation : class, IQueueHandler<TEntity>
        {
            services.AddSingleton<IQueueHandler<TEntity>, TImplementation>();
            services.AddHostedService<QueueConsumer<TEntity>>();

            return services;
        }
    }
}