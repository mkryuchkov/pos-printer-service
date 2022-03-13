using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using mkryuchkov.PosPrinter.Service.Core;

namespace mkryuchkov.PosPrinter.Service.Queue
{
    public class QueueBase<TEntity> : IQueue<TEntity>
    {
        private readonly ILogger<QueueBase<TEntity>> _logger;
        private readonly Channel<TEntity> _queue;

        public QueueBase(
            ILogger<QueueBase<TEntity>> logger,
            int capacity = 100)
        {
            _logger = logger;
            _queue = Channel.CreateBounded<TEntity>(new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            });
        }

        public async Task Enqueue(TEntity entity, CancellationToken cancellationToken)
        {
            await _queue.Writer.WriteAsync(entity, cancellationToken);

            _logger.LogDebug("Text {0} enqueued. Queue size: {1}", entity, _queue.Reader.Count);
        }

        public async Task<TEntity> Dequeue(CancellationToken cancellationToken)
        {
            var entity = await _queue.Reader.ReadAsync(cancellationToken);

            _logger.LogDebug("Text {0} dequeued. Queue size {1}", entity, _queue.Reader.Count);

            return entity;
        }
    }
}