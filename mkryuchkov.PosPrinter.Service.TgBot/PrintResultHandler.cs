using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using mkryuchkov.PosPrinter.Localization;
using mkryuchkov.PosPrinter.Model.Core;
using mkryuchkov.PosPrinter.Service.Core;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace mkryuchkov.PosPrinter.Service.TgBot;

public sealed class PrintResultHandler : IQueueHandler<PrintResult<MessageInfo>>
{
    private readonly ILogger<PrintResultHandler> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly ISharedStringLocalizer _localizer;

    public PrintResultHandler(
        ILogger<PrintResultHandler> logger,
        ITelegramBotClient botClient,
        ISharedStringLocalizer localizer)
    {
        _logger = logger;
        _botClient = botClient;
        _localizer = localizer;
    }

    public async Task HandleAsync(PrintResult<MessageInfo> result, CancellationToken token)
    {
        await _botClient.SendTextMessageAsync(
            result.Info!.ChatId,
            result.Success
                ? _localizer[result.Info!.LanguageCode, "Printed!"]
                : _localizer[result.Info!.LanguageCode, "ErrorWithFormat", result.ErrorData ?? string.Empty],
            parseMode: ParseMode.Markdown,
            replyToMessageId: result.Info.MesageId,
            cancellationToken: token
        );
    }
}
