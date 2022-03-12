using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESCPOS_NET;
using ESCPOS_NET.Utilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using mkryuchkov.ESCPOS.Goojprt;
using mkryuchkov.PosPrinter.Service.Core;

namespace mkryuchkov.PosPrinter.PrintService
{
    public class PrintService : BackgroundService
    {
        private static readonly Encoding Cp866 =
            CodePagesEncodingProvider.Instance.GetEncoding(866);
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

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(PrintService)} is stopping.");
            return base.StopAsync(cancellationToken);
        }

        private async Task PrintItem(string item, CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    _logger.LogDebug($"Printing {item}");

                    using var printer = new SerialPrinter("COM4", 9600);
                    var emitter = new Pt210();
                    printer.Write(ByteSplicer.Combine(
                        emitter.Initialize(),
                        emitter.Print(item, Cp866),
                        emitter.FeedLines(3)
                    ));

                    await Task.Delay(5000, cancellationToken);

                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error trying to print {item}");
                }

                await Task.Delay(5000, cancellationToken); // next try delay
            }

            _logger.LogDebug($"Printed {item}");
        }
    }
}