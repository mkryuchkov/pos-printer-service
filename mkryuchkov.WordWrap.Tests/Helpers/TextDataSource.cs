using System.Collections;

namespace mkryuchkov.WordWrap.Tests.Helpers
{
    internal sealed class TextDataSource : IEnumerable<object[]>
    {
        private const string Folder = "./Texts";

        private static readonly int[] Widths = { 8, 10, 16, 20, 24, 32, 80 };

        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var file in Directory.GetFiles(Folder))
            {
                using var reader = File.OpenText(file);
                var content = reader.ReadToEnd();
                foreach (var width in Widths)
                {
                    yield return new object[] { content, width };
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}