using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using mkryuchkov.TgBot;
using mkryuchkov.PosPrinter.Service.Core;

namespace mkryuchkov.PosPrinter.Service.TgBot
{
    public class PosPrinterBot : ITgUpdateHandler
    {
        private readonly ILogger<PosPrinterBot> _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly IPrintQueue _printQueue;

        public PosPrinterBot(
            ILogger<PosPrinterBot> logger,
            ITelegramBotClient botClient,
            IPrintQueue printQueue)
        {
            _logger = logger;
            _botClient = botClient;
            _printQueue = printQueue;
        }

        public async Task Handle(Update update, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Update received. Type: {update.Type}");

            if (update.Type == UpdateType.Message)
            {
                await _botClient.SendTextMessageAsync(update.Message!.Chat.Id,
                    $"Hi. Your message:\n```{update.Message.Text}```",
                    ParseMode.Markdown, cancellationToken: cancellationToken);
                await _printQueue.Enqueue(update.Message.Text, cancellationToken);
            }
            else
            {
                await _botClient.SendTextMessageAsync(update.Message!.Chat.Id,
                    $"Hi. Your update:\n```{JsonSerializer.Serialize(update)}```",
                    ParseMode.Markdown, cancellationToken: cancellationToken);
            }
        }
    }
}