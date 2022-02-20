using System.Threading;
using System.Threading.Tasks;

namespace mkryuchkov.PosPrinter.Service.Core
{
    public interface IPrintQueue
    {
        ValueTask Enqueue(string text, CancellationToken cancellationToken);
        ValueTask<string> Dequeue(CancellationToken cancellationToken);
    }
}