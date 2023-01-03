using mkryuchkov.PosPrinter.Model.Core;
using Telegram.Bot.Types;

namespace mkryuchkov.PosPrinter.Service.TgBot;

public static class TgUpdateExtensions
{
    public static PrintQuery<MessageInfo> GetPrintQuery(this Message message)
    {
        return new PrintQuery<MessageInfo>
        {
            Type = PrintQueryType.Text, // todo: delete Type, use both Image and Text + formatting
            Text = message.Text,
            Info = new()
            {
                ChatId = message.Chat.Id,
                MesageId = message.MessageId,
                Author = message.From.Username,
                Time = message.Date
            }
        };
    }
}