namespace mkryuchkov.PosPrinter.Model.Core
{
    public class PrintQuery<TId> : IPrintQuery<TId>
    {
        public TId Id { get; init; }
        public PrintQueryType Type { get; set; }
        public string? Text { get; set; }
        public byte[]? Image { get; set; }
    }
}