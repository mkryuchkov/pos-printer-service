using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using mkryuchkov.PosPrinter.Model.Core;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using mkryuchkov.TgBot;
using mkryuchkov.PosPrinter.Service.Core;
using mkryuchkov.PosPrinter.Common;

namespace mkryuchkov.PosPrinter.Service.TgBot
{
    public sealed class TgUpdateHandler : ITgUpdateHandler
    {
        private readonly ILogger<TgUpdateHandler> _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly IQueue<PrintQuery<MessageInfo>> _queue;

        public TgUpdateHandler(
            ILogger<TgUpdateHandler> logger,
            ITelegramBotClient botClient,
            IQueue<PrintQuery<MessageInfo>> queue)
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
                await _botClient.SendTextMessageAsync(
                    update.Message!.Chat.Id,
                    "Gonna print it!",
                    ParseMode.Markdown,
                    replyToMessageId: update.Message.MessageId,
                    cancellationToken: cancellationToken);

                await _queue.EnqueueAsync(update.Message.GetPrintQuery(), cancellationToken);
            }
            else
            {
                await _botClient.SendTextMessageAsync(
                    update.Message!.Chat.Id,
                    $"Can`t process this:\n```{update.ToJson()}```",
                    ParseMode.Markdown,
                    cancellationToken: cancellationToken);
            }
        }
    }
}