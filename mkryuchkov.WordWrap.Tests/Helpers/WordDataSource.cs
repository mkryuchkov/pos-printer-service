using System.Collections;

namespace mkryuchkov.WordWrap.Tests.Helpers;

internal sealed class WordDataSource : IEnumerable<object[]>
{
    private readonly IEnumerable<object[]> _data = new []
    {
        new object[] { "Data.", 10, "Data." },
        new object[] { "Data.", 5, "Data." },
        new object[] { "Datum", 5, "Datum" },
        new object[] { "Data.\n", 10, "Data.\n" },
        new object[] { "\nData.", 10, "\nData." },
        new object[] { "Datum.", 5, "Dat-\num." },
        new object[] { "SuperData.", 8, "SuperDa-\nta." },
        new object[] { "SuperData.", 9, "SuperDa-\nta." },
        new object[] { "Data!!!", 5, "Data!\n!!" },
        new object[] { "Datum!!!", 5, "Dat-\num!!!" },
        new object[] { "Data.Data", 5, "Data.\nData" },
        new object[] { "Data. Data", 5, "Data.\nData" },
        new object[] { "Data-Data", 5, "Data-\nData" },
        new object[] { "Data012345", 5, "Data0\n12345" },
        new object[] { "Data Data Data", 13, "Data Data Da-\nta" },
    };

    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}