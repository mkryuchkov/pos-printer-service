using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using mkryuchkov.PosPrinter.Model.Core;
using mkryuchkov.PosPrinter.Service.Core;
using mkryuchkov.PosPrinter.Service.Printing.Configuration;

namespace mkryuchkov.PosPrinter.Service.Printing
{
    public sealed class PrintQueryHandler : IQueueHandler<IPrintQuery<int>>
    {
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

        public async Task HandleAsync(IPrintQuery<int> query, CancellationToken token)
        {
            var tryCount = _config.RetryMaxCount;
            Exception lastException = null;
            while (tryCount-- > 0)
            {
                try
                {
                    _logger.LogDebug("Printing {query}", query);

                    using var printer = _config.GetPrinter();
                    printer.Write(query.GetPrintCommands());

                    await Task.Delay(_config.RetryDelayMs, token);

                    break;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    _logger.LogError(ex, "Can't print {query}", query);
                    lastException = ex;

                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error trying to print {query}", query);
                    lastException = ex;
                    await Task.Delay(_config.RetryDelayMs, token);
                }
            }

            await _resultQueue.EnqueueAsync(new PrintResult<int>
            {
                Id = query.Id,
                Success = lastException == null,
                ErrorData = lastException?.Message
            }, token);

            _logger.LogDebug("Printed {query}", query);
        }
    }
}