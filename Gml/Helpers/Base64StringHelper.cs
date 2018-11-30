using System;
using System.Text;

namespace Gml.Helpers
{
    public static class Base64StringHelper
    {
        public static string Decode(string text, Encoding encoding = null)
        {
            var bytes = DecodeBytes(text);
            return encoding != null
                ? encoding.GetString(bytes)
                : Encoding.UTF8.GetString(bytes);
        }

        public static byte[] DecodeBytes(string text)
        {
            return Convert.FromBase64String(
                text.Replace("_", "/")
                    .Replace("-", "+")
                    .Replace("*", "=")
            );
        }

        public static string Encode(string text, Encoding encoding = null)
        {
            var bytes = encoding != null
                ? encoding.GetBytes(text)
                : Encoding.UTF8.GetBytes(text);
            return Convert
                .ToBase64String(bytes)
                .Replace("/", "_")
                .Replace("+", "-")
                .Replace("=", string.Empty);
        }
    }
}