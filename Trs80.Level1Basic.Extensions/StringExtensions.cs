using System;
using System.Linq;

namespace Trs80.Level1Basic.Extensions
{
    public static class StringExtensions
    {
        public static string ToPascalCase(this string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (text.Length == 1) return text.ToUpper();

            string[] words = text.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length > 1)
                return string.Concat(words.Select(word => word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower()));

            return words[0].Substring(0, 1).ToUpper() + words[0].Substring(1);
        }
        public static string ToCamelCase(this string text)
        {
            string result = ToPascalCase(text);

            return result.Substring(0, 1).ToLower() + result.Substring(1);
        }
    }
}
