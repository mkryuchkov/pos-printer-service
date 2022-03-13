using System.Threading;
using System.Threading.Tasks;

namespace mkryuchkov.PosPrinter.Service.Core
{
    public interface IQueue<TEntity>
    {
        Task Enqueue(TEntity entity, CancellationToken cancellationToken);

        Task<TEntity> Dequeue(CancellationToken cancellationToken);
    }
}