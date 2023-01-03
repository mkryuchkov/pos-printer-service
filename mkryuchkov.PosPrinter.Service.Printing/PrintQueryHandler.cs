using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using mkryuchkov.PosPrinter.Common;
using mkryuchkov.PosPrinter.Model.Core;
using mkryuchkov.PosPrinter.Service.Core;
using mkryuchkov.PosPrinter.Service.Printing.Configuration;

namespace mkryuchkov.PosPrinter.Service.Printing
{
    public sealed class PrintQueryHandler : IQueueHandler<PrintQuery<MessageInfo>>
    {
        private readonly ILogger<PrintQueryHandler> _logger;
        private readonly PrinterConfig _config;
        private readonly IQueue<PrintResult<MessageInfo>> _resultQueue;

        public PrintQueryHandler(
            ILogger<PrintQueryHandler> logger,
            IOptions<PrinterConfig> config,
            IQueue<PrintResult<MessageInfo>> resultQueue)
        {
            _logger = logger;
            _config = config.Value;
            _resultQueue = resultQueue;
        }

        public async Task HandleAsync(PrintQuery<MessageInfo> query, CancellationToken token)
        {
            using var scope = _logger.BeginScope("queryId", query.Id);
            _logger.LogDebug("Printing {query}", query.ToJson());

            var tryCount = _config.RetryMaxCount;
            Exception lastException = null;
            while (tryCount > 0)
            {
                try
                {
                    using (var printer = _config.GetPrinter())
                    {
                        printer.Write(query.GetPrintCommands());

                        await Task.Delay(_config.RetryDelayMs, token);
                    }

                    lastException = null;
                    break;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    _logger.LogError(ex, "Can't print");
                    lastException = ex;

                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error trying to print ({count}).", tryCount);
                    lastException = ex;
                    await Task.Delay(_config.RetryDelayMs, token);
                }
                tryCount -= 1;
            }

            await _resultQueue.EnqueueAsync(new PrintResult<MessageInfo>
            {
                Id = query.Id,
                Success = lastException == null,
                ErrorData = lastException?.Message,
                Info = query.Info
            }, token);

            _logger.LogDebug("Printed.");
        }
    }
}