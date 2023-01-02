using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESCPOS_NET;
using ESCPOS_NET.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using mkryuchkov.ESCPOS.Goojprt;
using mkryuchkov.PosPrinter.Model.Core;
using mkryuchkov.PosPrinter.Service.Core;
using mkryuchkov.PosPrinter.Service.Printing.Configuration;

namespace mkryuchkov.PosPrinter.Service.Printing
{
    public sealed class PrintQueryHandler : IQueueHandler<IPrintQuery<int>>
    {
        private static readonly Encoding Cp866 =
            CodePagesEncodingProvider.Instance.GetEncoding(866);

        private readonly ILogger<PrintQueryHandler> _logger;
        private readonly PrinterConfig _config;
        private readonly IQueue<IPrintResult<int>> _resultQueue;

        public PrintQueryHandler(
            ILogger<PrintQueryHandler> logger,
            IOptions<PrinterConfig> config,
            IQueue<IPrintResult<int>> resultQueue)
        {
            _logger = logger;
            _config = config.Value;
            _resultQueue = resultQueue;
        }

        public async Task HandleAsync(IPrintQuery<int> item, CancellationToken token)
        {
            while (true) // todo: retry policy (polly?)
            {
                try
                {
                    _logger.LogDebug("Printing {item}", item);

                    if (item.Type == PrintQueryType.Text) // todo: other types // refactor
                    {
                        using var printer = new SerialPrinter("COM4", 9600); // todo: settings for printer
                        var emitter = new Pt210();
                        printer.Write(ByteSplicer.Combine(
                            emitter.Initialize(),
                            emitter.Print(item.Text, Cp866),
                            emitter.FeedLines(3),
                            emitter.Beep()
                        ));

                        await Task.Delay(5000, token);
                    }

                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error trying to print {item}", item);
                }

                await Task.Delay(5000, token); // next try delay
            }

            await _resultQueue.EnqueueAsync(new PrintResult<int>
            {
                Id = item.Id,
                Success = true
            }, token);

            _logger.LogDebug("Printed {item}", item);
        }
    }
}