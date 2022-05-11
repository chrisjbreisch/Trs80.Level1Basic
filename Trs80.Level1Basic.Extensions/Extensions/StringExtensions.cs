using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Trs80.Level1Basic.Common.Extensions;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class StringExtensions
{
    public static string ToPascalCase(this string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        if (text.Length == 1) return text.ToUpper();

        string[] words = text.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

        if (words.Length > 1)
            return string.Concat(words.Select(word => word[..1].ToUpper() + word[1..].ToLower()));

        return words[0][..1].ToUpper() + words[0][1..];
    }

    public static string ToCamelCase(this string text)
    {
        string result = ToPascalCase(text);

        return result[..1].ToLower() + result[1..];
    }
}