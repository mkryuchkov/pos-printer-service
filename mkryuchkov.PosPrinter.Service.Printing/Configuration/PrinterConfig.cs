namespace mkryuchkov.PosPrinter.Service.Printing.Configuration
{
    public class PrinterConfig
    {
        public PrinterType Type { get; init; }
        public string Location { get; init; }
        public int BaudRate { get; init; } = Const.DefaultBaudRate;
        public int RetryCount { get; init; }
    }
}
