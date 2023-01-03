using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using mkryuchkov.PosPrinter.Common;
using mkryuchkov.PosPrinter.Model.Core;
using mkryuchkov.PosPrinter.Service.Core;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace mkryuchkov.PosPrinter.Service.TgBot;

public sealed class PrintResultHandler : IQueueHandler<PrintResult<MessageInfo>>
{
    private readonly ILogger<PrintResultHandler> _logger;
    private readonly ITelegramBotClient _botClient;

    public PrintResultHandler(
        ILogger<PrintResultHandler> logger,
        ITelegramBotClient botClient)
    {
        _logger = logger;
        _botClient = botClient;
    }

    public async Task HandleAsync(PrintResult<MessageInfo> result, CancellationToken token)
    {
        await _botClient.SendTextMessageAsync(
            result.Info.ChatId,
#if DEBUG
            $"Print result:\n```\n{result.ToJson()}\n```",
#else
            result.Success
                ? "Printed!"
                : $"Error happened:\n```\n{ErrorData}\n```";
#endif
            ParseMode.Markdown,
            replyToMessageId: result.Info.MesageId,
            cancellationToken: token
        );
    }
}
