using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using mkryuchkov.PosPrinter.Service.Core;

namespace mkryuchkov.PosPrinter.Service.Queue;

public sealed class QueueConsumer<TEntity> : BackgroundService
{
    private readonly ILogger<QueueConsumer<TEntity>> _logger;
    private readonly IQueue<TEntity> _queue;
    private readonly IQueueHandler<TEntity> _handler;

    public QueueConsumer(
        ILogger<QueueConsumer<TEntity>> logger,
        IQueue<TEntity> queue,
        IQueueHandler<TEntity> handler)
    {
        _logger = logger;
        _queue = queue;
        _handler = handler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug($"Starting {nameof(QueueConsumer<TEntity>)}");

        while (!stoppingToken.IsCancellationRequested)
        {
            var entity = await _queue.DequeueAsync(stoppingToken);

            try
            {
                await _handler.HandleAsync(entity, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error hadlning entity {entity}", JsonSerializer.Serialize(entity));
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Stopping {nameof(QueueConsumer<TEntity>)}");
        return base.StopAsync(cancellationToken);
    }
}
