namespace mkryuchkov.WordWrap.Tests;

internal static class StringExtensions
{
    private static readonly char[] Additions = { ' ', '\n', '-' };
    
    public static string RemoveAdditions(this string str)
    {
        return string.Join("", str.Split(Additions, StringSplitOptions.RemoveEmptyEntries));
    } 
}