using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace mkryuchkov.TgBot
{
    public interface ITgUpdateHandler
    {
        Task Handle(Update update, CancellationToken cancellationToken);
    }
}