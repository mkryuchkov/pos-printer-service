using Microsoft.Extensions.Logging;
using mkryuchkov.PosPrinter.Model.Core;

namespace mkryuchkov.PosPrinter.Service.Queue
{
    public class PrintQueryQueue : QueueBase<PrintQuery<MessageInfo>>
    {
        public PrintQueryQueue(
            ILogger<PrintQueryQueue> logger,
            int capacity = 100)
            : base(logger, capacity)
        {
        }
    }
}