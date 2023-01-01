using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using mkryuchkov.PosPrinter.Model.Core;
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
        private readonly IQueue<IPrintQuery<int>> _queue;

        public PosPrinterBot(
            ILogger<PosPrinterBot> logger,
            ITelegramBotClient botClient,
            IQueue<IPrintQuery<int>> queue)
        {
            _logger = logger;
            _botClient = botClient;
            _queue = queue;
        }

        public async Task Handle(Update update, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update received. Type: {type}", update.Type);

            if (update.Type == UpdateType.Message)
            {
                await _botClient.SendTextMessageAsync(update.Message!.Chat.Id,
                    $"Hi. Your message:\n```{update.Message.Text}```",
                    ParseMode.Markdown, cancellationToken: cancellationToken);
                await _queue.EnqueueAsync(new PrintQuery<int>
                {
                    Id = update.Message.MessageId,
                    Type = PrintQueryType.Text,
                    Text = update.Message.Text
                }, cancellationToken);
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