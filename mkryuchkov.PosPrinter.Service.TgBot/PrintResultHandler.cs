using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using mkryuchkov.PosPrinter.Model.Core;
using mkryuchkov.PosPrinter.Service.Core;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace mkryuchkov.PosPrinter.Service.TgBot;

public sealed class PrintResultHandler : IQueueHandler<IPrintResult<int>>
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

    public async Task HandleAsync(IPrintResult<int> result, CancellationToken token)
    {
        await _botClient.SendTextMessageAsync(result.Id,
                    $"Print result:\n```{JsonSerializer.Serialize(result)}```",
                    ParseMode.Markdown, cancellationToken: token);
    }
}
