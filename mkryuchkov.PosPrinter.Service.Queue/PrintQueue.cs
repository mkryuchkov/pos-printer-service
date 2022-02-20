using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using mkryuchkov.PosPrinter.Service.Core;

namespace mkryuchkov.PosPrinter.Service.Queue
{
    public class PrintQueue : IPrintQueue
    {
        private readonly ILogger<PrintQueue> _logger;
        private readonly Channel<string> _queue;

        public PrintQueue(
            ILogger<PrintQueue> logger,
            int capacity = 100)
        {
            _logger = logger;
            _queue = Channel.CreateBounded<string>(new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            });
        }

        public async ValueTask Enqueue(string text, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                // throw new ArgumentException(nameof(text));
                _logger.LogWarning("Empty message. Ignoring.");
                return;
            }

            await _queue.Writer.WriteAsync(text, cancellationToken);

            _logger.LogDebug("Text {0} enqueued. Queue size: {1}", text, _queue.Reader.Count);
        }

        public async ValueTask<string> Dequeue(CancellationToken cancellationToken)
        {
            var text = await _queue.Reader.ReadAsync(cancellationToken);

            _logger.LogDebug("Text {0} dequeued. Queue size {1}", text, _queue.Reader.Count);

            return text;
        }
    }
}