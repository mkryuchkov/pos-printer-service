namespace mkryuchkov.PosPrinter.Model.Core
{
    public interface IPrintResult<TId>
    {
        public TId Id { get; init; }
        
        public bool Success { get; set; }
        
        public string? ErrorData { get; set; }
    }
}