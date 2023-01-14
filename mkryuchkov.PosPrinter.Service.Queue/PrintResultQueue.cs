using Microsoft.Extensions.Logging;
using mkryuchkov.PosPrinter.Model.Core;

namespace mkryuchkov.PosPrinter.Service.Queue
{
    public class PrintResultQueue : QueueBase<PrintResult<MessageInfo>>
    {
        public PrintResultQueue(
            ILogger<PrintResultQueue> logger,
            int capacity = 100)
            : base(logger, capacity)
        {
        }
    }
}