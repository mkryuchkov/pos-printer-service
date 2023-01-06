namespace mkryuchkov.PosPrinter.Model.Core
{
    public sealed class MessageInfo
    {
        public long ChatId { get; init; }
        public int? MesageId { get; init; }
        public string? Author { get; init; }
        public DateTime Time { get; init; }
        public string? LanguageCode { get; init; }
    }
}