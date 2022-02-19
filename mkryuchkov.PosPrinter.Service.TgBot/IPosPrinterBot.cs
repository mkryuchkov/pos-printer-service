using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace mkryuchkov.PosPrinter.TgBot
{
    public interface IPosPrinterBot
    {
        Task HandleTgUpdate(Update update);
    }
}