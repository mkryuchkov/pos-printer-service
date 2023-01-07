using System.Text.Json;

namespace mkryuchkov.PosPrinter.Common
{
    public static class SerializationExtensions
    {
        public static string ToJson(this object obj) => JsonSerializer.Serialize(obj);
    }
}