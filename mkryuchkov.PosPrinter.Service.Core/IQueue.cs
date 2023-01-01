using System.Threading;
using System.Threading.Tasks;

namespace mkryuchkov.PosPrinter.Service.Core
{
    public interface IQueue<TEntity>
    {
        Task EnqueueAsync(TEntity entity, CancellationToken cancellationToken);

        Task<TEntity> DequeueAsync(CancellationToken cancellationToken);
    }
}