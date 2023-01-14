namespace mkryuchkov.PosPrinter.Model.Core
{
    public sealed class PrintResult<TInfo>
    {
        public Guid Id { get; init; }
        public bool Success { get; set; }
        public string? ErrorData { get; set; }
        public TInfo? Info { get; set; }
    }
}