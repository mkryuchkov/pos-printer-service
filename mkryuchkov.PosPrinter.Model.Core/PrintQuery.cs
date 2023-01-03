namespace mkryuchkov.PosPrinter.Model.Core
{
    public sealed class PrintQuery<TInfo>
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public PrintQueryType Type { get; set; }
        public string? Text { get; set; }
        public byte[]? Image { get; set; }

        public TInfo? Info { get; set; }
    }
}