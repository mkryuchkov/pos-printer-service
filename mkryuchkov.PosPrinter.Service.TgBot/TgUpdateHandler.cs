using System.IO;
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
using System.Linq;

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

            // todo: if photo or text exists
            if (update.Type == UpdateType.Message)
            {
                // todo: localization of messages
                await _botClient.SendTextMessageAsync(
                    update.Message!.Chat.Id,
                    "Gonna print it!",
                    ParseMode.Markdown,
                    replyToMessageId: update.Message.MessageId,
                    cancellationToken: cancellationToken);

                await _queue.EnqueueAsync(
                    await GetPrintQueryAsync(update.Message, cancellationToken),
                    cancellationToken);
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

        private async Task<PrintQuery<MessageInfo>> GetPrintQueryAsync(Message message, CancellationToken token)
        {
            return new PrintQuery<MessageInfo>
            {
                Text = message.Text,
                Image = await GetPhotoAsync(message, token), // todo: photo caption
                Info = new()
                {
                    ChatId = message.Chat.Id,
                    MesageId = message.MessageId,
                    Author = message.From.Username,
                    Time = message.Date
                }
            };
        }

        private const long TwentyMiBs = 20 * 1024 * 1024;

        private async Task<byte[]> GetPhotoAsync(Message message, CancellationToken token)
        {
            if (message.Photo != null && message.Photo.Length > 0)
            {
                var photoId = message.Photo
                    .Where(p => p.FileSize < TwentyMiBs)
                    .MaxBy(p => p.FileSize)
                    .FileId;
                using (var stream = new MemoryStream())
                {
                    await _botClient.GetInfoAndDownloadFileAsync(photoId, stream, token);
                    return stream.ToArray();
                }
            }

            return null;
        }
    }
}