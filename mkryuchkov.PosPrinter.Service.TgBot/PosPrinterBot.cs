using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace mkryuchkov.PosPrinter.TgBot
{
    public class PosPrinterBot : IPosPrinterBot
    {
        private readonly ILogger<PosPrinterBot> _logger;
        private readonly ITelegramBotClient _botClient;

        public PosPrinterBot(
            ILogger<PosPrinterBot> logger,
            ITelegramBotClient botClient)
        {
            _logger = logger;
            _botClient = botClient;
        }

        public async Task HandleTgUpdate(Update update)
        {
            _logger.LogInformation($"Update received. Type: {update.Type}");

            if (update.Type == UpdateType.Message)
            {
                await _botClient.SendTextMessageAsync(update.Message!.Chat.Id,
                        $"Hi. Your message {update.Message.Text}");
            }
            else
            {
                await _botClient.SendTextMessageAsync(update.Message!.Chat.Id,
                    $"Hi. Your update ```{JsonSerializer.Serialize(update)}```");
            }
        }
    }
}