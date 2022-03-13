using Microsoft.Extensions.Logging;
using mkryuchkov.PosPrinter.Model.Core;

namespace mkryuchkov.PosPrinter.Service.Queue
{
    public class PrintResultQueue : QueueBase<IPrintResult<int>>
    {
        public PrintResultQueue(
            ILogger<QueueBase<IPrintResult<int>>> logger,
            int capacity = 100)
            : base(logger, capacity)
        {
        }
    }
}