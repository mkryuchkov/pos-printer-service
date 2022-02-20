using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using mkryuchkov.PosPrinter.Service.Core;

namespace mkryuchkov.PosPrinter.PrintService
{
    public class PrintService : BackgroundService
    {
        private readonly ILogger<PrintService> _logger;
        private readonly IPrintQueue _printQueue;

        public PrintService(
            ILogger<PrintService> logger,
            IPrintQueue printQueue)
        {
            _logger = logger;
            _printQueue = printQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(PrintService)} is started.");
            await BackgroundPrinting(cancellationToken);
        }

        private async Task BackgroundPrinting(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var item = await _printQueue.Dequeue(cancellationToken);

                try
                {
                    await PrintItem(item, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error printing item {0}", item);
                }
            }
        }

        private Task PrintItem(string item, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"PRINTING {item}");
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(PrintService)} is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}