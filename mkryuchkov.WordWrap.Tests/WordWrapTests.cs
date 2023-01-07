using System.ComponentModel;
using FluentAssertions;
using mkryuchkov.WordWrap.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace mkryuchkov.WordWrap.Tests
{
    public class WordWrapTests
    {
        private readonly ITestOutputHelper _output;
        private readonly Func<string, int, string> _sut;

        public WordWrapTests(ITestOutputHelper output)
        {
            _output = output;
            _sut = WordWrapExtensions.Wrap;
        }

        [Theory]
        [ClassData(typeof(TextDataSource))]
        public void TestOnTexts_Success(string text, int width)
        {
            var result = _sut(text, width);

            _output.WriteLine(result);

            result.RemoveAdditions().Should().BeEquivalentTo(text.RemoveAdditions());

            result.Split('\n')
                .Should().AllSatisfy(line =>
                    line.Length.Should().BeLessThanOrEqualTo(width));
        }

        [Theory]
        [ClassData(typeof(WordDataSource))]
        public void TestOnWords_Success(string source, int width, string expected)
        {
            var result = _sut(source, width);

            _output.WriteLine(result);

            result.Should().BeEquivalentTo(expected);
        }
    }
}