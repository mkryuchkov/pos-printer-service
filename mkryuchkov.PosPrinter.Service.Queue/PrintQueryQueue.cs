using Microsoft.Extensions.Logging;
using mkryuchkov.PosPrinter.Model.Core;

namespace mkryuchkov.PosPrinter.Service.Queue
{
    public class PrintQueryQueue : QueueBase<IPrintQuery<int>>
    {
        public PrintQueryQueue(
            ILogger<QueueBase<IPrintQuery<int>>> logger,
            int capacity = 100)
            : base(logger, capacity)
        {
        }
    }
}