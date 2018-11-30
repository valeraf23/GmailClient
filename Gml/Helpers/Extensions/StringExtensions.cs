using System.Text.RegularExpressions;

namespace Gml.Helpers.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceWhitespaces(this string input, string replaceWith)
        {
            return Regex.Replace(input, @"\s+", replaceWith);
        }

        public static string ReplaceWhitespaces(this string input)
        {
            return Regex.Replace(input, @"\s+", string.Empty);
        }
    }
}