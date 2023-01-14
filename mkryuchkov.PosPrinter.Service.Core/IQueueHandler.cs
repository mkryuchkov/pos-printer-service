using System.Threading;
using System.Threading.Tasks;

namespace mkryuchkov.PosPrinter.Service.Core
{
    public interface IQueueHandler<TEntity>
    {
        Task HandleAsync(TEntity entity, CancellationToken token);
    }
}