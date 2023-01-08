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
using System.Linq;
using mkryuchkov.PosPrinter.Localization;

namespace mkryuchkov.PosPrinter.Service.TgBot
{
    public sealed class TgUpdateHandler : ITgUpdateHandler
    {
        private readonly ILogger<TgUpdateHandler> _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly IQueue<PrintQuery<MessageInfo>> _queue;
        private readonly ISharedStringLocalizer _localizer;

        public TgUpdateHandler(
            ILogger<TgUpdateHandler> logger,
            ITelegramBotClient botClient,
            IQueue<PrintQuery<MessageInfo>> queue,
            ISharedStringLocalizer localizer)
        {
            _logger = logger;
            _botClient = botClient;
            _queue = queue;
            _localizer = localizer;
        }

        public async Task Handle(Update update, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update received. Type: {type}", update.Type);

            // todo: whitelist or something
            // todo: max length for text // max height for image
            // todo: /start /help etc.

            if (update.Type == UpdateType.Message
                && ((update.Message!.Sticker != null && !update.Message.Sticker.IsAnimated && !update.Message.Sticker.IsVideo)
                    || update.Message!.Text != null
                    || update.Message.Photo != null))
            {
                await _botClient.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    _localizer[update.Message.From?.LanguageCode, "Gonna print it!"],
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
                    _localizer[update.Message.From?.LanguageCode, "Can't process this."],
                    replyToMessageId: update.Message.MessageId,
                    cancellationToken: cancellationToken);
            }
        }

        private async Task<PrintQuery<MessageInfo>> GetPrintQueryAsync(Message message, CancellationToken token)
        {
            return new PrintQuery<MessageInfo>
            {
                Text = message.Text,
                Image = await GetImageAsync(message, token),
                Caption = message.Caption,
                Info = new MessageInfo
                {
                    ChatId = message.Chat.Id,
                    MesageId = message.MessageId,
                    Author = GetAuthor(message.From),
                    LanguageCode = message.From?.LanguageCode,
                    Time = message.Date
                }
            };
        }

        private const long TwentyMiBs = 20 * 1024 * 1024;

        private async Task<byte[]?> GetImageAsync(Message message, CancellationToken token)
        {
            string? imageId = null;

            if (message.Photo != null && message.Photo.Length > 0)
            {
                imageId = message.Photo
                    .Where(p => p.FileSize < TwentyMiBs)
                    .MaxBy(p => p.FileSize)!
                    .FileId;
            }

            if (message.Sticker != null && !message.Sticker.IsAnimated && !message.Sticker.IsVideo)
            {
                imageId = message.Sticker.FileId;
            }

            if (imageId == null)
            {
                return null;
            }

            using var stream = new MemoryStream();
            await _botClient.GetInfoAndDownloadFileAsync(imageId, stream, token);
            return stream.ToArray();
        }

        private static string GetAuthor(User? user)
        {
            return $"{user?.FirstName} ({user?.Username})";
        }
    }
}