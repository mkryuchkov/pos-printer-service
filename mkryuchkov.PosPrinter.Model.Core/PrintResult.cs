namespace mkryuchkov.PosPrinter.Model.Core
{
    public class PrintResult<TId> : IPrintResult<TId>
    {
        public TId Id { get; init; }
        public bool Success { get; set; }
        public string? ErrorData { get; set; }
    }
}