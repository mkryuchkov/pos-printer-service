namespace mkryuchkov.PosPrinter.Service.Printing.Configuration
{
    public class PrinterConfig
    {
        public PrinterType Type { get; init; }
        public string? Location { get; init; }
        public int BaudRate { get; init; } = 9600;
        public int RetryMaxCount { get; init; } = 3;
        public int RetryDelayMs { get; init; } = 5000;
        public string? TimeZone { get; init; } = "Russian Standard Time";
    }
}
