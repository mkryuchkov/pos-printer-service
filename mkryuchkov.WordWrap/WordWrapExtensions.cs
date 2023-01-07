using System.Text;

namespace mkryuchkov.WordWrap;

public static class WordWrapExtensions
{
    public static string Wrap(this string source, int maxLineLength)
    {
        var sb = new StringBuilder();
        var firstLine = true;

        foreach (var (line, addHyphen) in ReduceIntoLines(source, maxLineLength))
        {
            if (!firstLine)
            {
                sb.AppendLine();
            }
            else
            {
                firstLine = false;
            }

            sb.AppendJoin(' ', line);
            if (addHyphen)
            {
                sb.Append('-');
            }
        }

        return sb.ToString();
    }

    private static IEnumerable<(IList<ReadOnlyMemory<char>> line, bool addHyphen)> ReduceIntoLines(string source,
        int maxLineLength)
    {
        var length = 0;
        var line = new List<ReadOnlyMemory<char>>();
        ReadOnlyMemory<char>? word = null;

        using var enumerator = MapIntoWords(source).GetEnumerator();

        while (true)
        {
            if (!word.HasValue)
            {
                if (!enumerator.MoveNext())
                {
                    break;
                }

                word = enumerator.Current;
            }

            if (word.Value.Length == 1 && word.Value.Span[0] == '\n')
            {
                yield return (line, false);

                line.Clear();
                length = 0;
                word = null;
                continue;
            }

            var newLength = length + word.Value.Length;

            if (newLength > maxLineLength)
            {
                if (TryHyphenateWord(word.Value, maxLineLength - length,
                        out var parts, out var needHyphen))
                {
                    line.Add(parts.first);
                    yield return (line, needHyphen);

                    line.Clear();
                    length = 0;
                    word = parts.second;
                    continue;
                }

                yield return (line, false);
                line.Clear();
                length = 0;
                continue;
            }

            line.Add(word.Value);
            word = null;
            length = newLength + 1; // plus space
        }

        yield return (line, false);
    }

    private static bool TryHyphenateWord(
        ReadOnlyMemory<char> word, int positions,
        out (ReadOnlyMemory<char> first, ReadOnlyMemory<char> second) parts,
        out bool needHyphen)
    {
        parts = (null, null);
        needHyphen = false;

        // at least 2 chars (plus hyphen) before 
        if (positions < 3 || word.Length < 4)
        {
            return false;
        }

        var index = positions;

        needHyphen = char.IsLetter(word.Span[positions - 1]);
        index -= (needHyphen ? 1 : 0);

        var leftLetter = char.IsLetter(word.Span[index]) && !char.IsLetter(word.Span[index + 1]);
        index -= leftLetter ? 1 : 0;

        parts = (word[..index], word[index..]);
        return true;
    }

    private static IEnumerable<ReadOnlyMemory<char>> MapIntoWords(string source)
    {
        var memory = source.AsMemory();
        var start = 0;
        var end = 0;

        while (end < source.Length)
        {
            if (source[end] == ' ' || source[end] == '\n')
            {
                if (start < end)
                {
                    yield return memory[start..end];
                }

                if (source[end] == '\n')
                {
                    yield return memory[end..(end + 1)];
                }

                start = end + 1;
            }

            end += 1;
        }

        yield return memory[start..end];
    }
}