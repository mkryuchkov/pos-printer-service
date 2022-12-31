using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESCPOS_NET;
using ESCPOS_NET.Utilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using mkryuchkov.ESCPOS.Goojprt;
using mkryuchkov.PosPrinter.Model.Core;
using mkryuchkov.PosPrinter.Service.Core;

namespace mkryuchkov.PosPrinter.PrintService
{
    public sealed class PrintService : BackgroundService
    {
        private static readonly Encoding Cp866 =
            CodePagesEncodingProvider.Instance.GetEncoding(866);
        private readonly ILogger<PrintService> _logger;
        private readonly IQueue<IPrintQuery<int>> _printQueue;
        private readonly IQueue<IPrintResult<int>> _resultQueue;

        public PrintService(
            ILogger<PrintService> logger,
            IQueue<IPrintQuery<int>> printQueue,
            IQueue<IPrintResult<int>> resultQueue)
        {
            _logger = logger;
            _printQueue = printQueue;
            _resultQueue = resultQueue;
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
                    _logger.LogError(ex, "Error printing item {item}", item);
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(PrintService)} is stopping.");
            return base.StopAsync(cancellationToken);
        }

        private async Task PrintItem(IPrintQuery<int> item, CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    _logger.LogDebug("Printing {item}", item);

                    if (item.Type == PrintQueryType.Text) // todo: other types // refactor
                    {
                        using var printer = new SerialPrinter("COM4", 9600);
                        var emitter = new Pt210();
                        printer.Write(ByteSplicer.Combine(
                            emitter.Initialize(),
                            emitter.Print(item.Text, Cp866),
                            emitter.FeedLines(3),
                            emitter.Beep()
                        ));

                        await Task.Delay(5000, cancellationToken);
                    }

                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error trying to print {item}", item);
                }

                await Task.Delay(5000, cancellationToken); // next try delay
            }

            await _resultQueue.Enqueue(new PrintResult<int>
            {
                Id = item.Id,
                Success = true
            }, cancellationToken);
            _logger.LogDebug("Printed {item}", item);
        }
    }
}