using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
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
    private readonly IStringLocalizer<Shared> _localizer;

    public PrintResultHandler(
        ILogger<PrintResultHandler> logger,
        ITelegramBotClient botClient,
        IStringLocalizer<Shared> localizer)
    {
        _logger = logger;
        _botClient = botClient;
        _localizer = localizer;
    }

    public async Task HandleAsync(PrintResult<MessageInfo> result, CancellationToken token)
    {
        result.Info!.LanguageCode.SetCurrentUICulture();

        await _botClient.SendTextMessageAsync(
            result.Info.ChatId,
#if !DEBUG
            $"Print result:\n```\n{result.ToJson()}\n```",
#else
            result.Success
                ? _localizer["Printed!"]
                : _localizer["ErrorWithFormat", result.ErrorData ?? string.Empty].ReEscape(),
#endif
        ParseMode.Markdown,
            replyToMessageId: result.Info.MesageId,
            cancellationToken: token
        );
    }
}
