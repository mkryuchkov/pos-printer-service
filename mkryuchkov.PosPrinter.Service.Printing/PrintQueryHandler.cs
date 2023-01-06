using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESCPOS_NET.Utilities;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using mkryuchkov.ESCPOS.Goojprt;
using mkryuchkov.PosPrinter.Common;
using mkryuchkov.PosPrinter.Localization;
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
        private readonly IStringLocalizer<Shared> _localizer;

        public PrintQueryHandler(
            ILogger<PrintQueryHandler> logger,
            IOptions<PrinterConfig> config,
            IQueue<PrintResult<MessageInfo>> resultQueue,
            IStringLocalizer<Shared> localizer)
        {
            _logger = logger;
            _config = config.Value;
            _resultQueue = resultQueue;
            _localizer = localizer;
        }

        public async Task HandleAsync(PrintQuery<MessageInfo> query, CancellationToken token)
        {
            using var scope = _logger.BeginScope("queryId", query.Id);
            _logger.LogInformation("Printing {query}", query.ToJson());

            var tryCount = _config.RetryMaxCount;
            Exception? lastException = null;
            while (tryCount > 0)
            {
                try
                {
                    using (var printer = _config.GetPrinter())
                    {
                        printer.Write(GetPrintCommands(query));

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
                    _logger.LogWarning(ex, "Error trying to print ({count}).", tryCount);
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

            if (lastException != null)
            {
                _logger.LogError(lastException, "Unprinted: {message}.", lastException.Message);
            }
            else
            {
                _logger.LogInformation("Printed.");
            }
        }

        private static readonly Encoding Cp866 =
            CodePagesEncodingProvider.Instance.GetEncoding(866)!;
        private static readonly Pt210 Emitter = new();

        private byte[] GetPrintCommands(PrintQuery<MessageInfo> query)
        {
            query.Info!.LanguageCode.SetCurrentUICulture();

            return ByteSplicer.Combine(
                Emitter.Initialize(),
                Emitter.Print(GetHeader(query.Info!), Cp866),
                Emitter.FeedDots(10),
                GetImageCommands(query.Image, query.Caption),
                query.Text != null
                    ? Emitter.Print(query.Text, Cp866)
                    : Array.Empty<byte>(),
                Emitter.FeedLines(3),
                Emitter.Beep()
            );
        }

        private byte[] GetImageCommands(byte[]? image, string? caption)
        {
            return ByteSplicer.Combine(
                image != null
                    ? Emitter.PrintImage(image)
                    : Array.Empty<byte>(),
                image != null && caption != null
                    ? Emitter.Print(caption, Cp866)
                    : Array.Empty<byte>(),
                image != null || caption != null
                    ? Emitter.FeedLines(1)
                    : Array.Empty<byte>()
            );
        }

        private string GetHeader(MessageInfo info)
        {
            return _localizer["HeaderWithFormat", info.Author!, FormatTimeToMsk(info.Time)].ReEscape();
        }

        // todo: parameters or localization
        private static string FormatTimeToMsk(DateTime time)
        {
            return TimeZoneInfo
                .ConvertTimeBySystemTimeZoneId(time, "Russian Standard Time")
                .ToString("HH:mm  dd.MM.yyyy");
        }
    }
}