using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Trs80.Level1Basic.Common.Extensions;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class StringExtensions
{
    public static string SeparateWordsByCase(this string text, char separatorChar)
    {
        StringBuilder sb = new();
        int start = 0;
        int end = 1;

        while (end < text.Length)
        {
            char c = text[end];
            if (char.IsUpper(c))
            {
                sb.Append(text[start..end]);
                sb.Append(separatorChar);
                start = end;
            }
            end++;
        }
        sb.Append(text[start..end]);

        return sb.ToString();
    }
    public static string ToPascalCase(this string text)
    {
        string result = HandleBaseCases(text, true);
        if (result == null) return string.Empty;
        if (!string.IsNullOrEmpty(result)) return result;

        string[] words = SplitWords(text);

        if (words.Length > 1)
            return string.Concat(words.Select(word => word[..1].ToUpper() + word[1..].ToLower()));

        return words[0][..1].ToUpper() + words[0][1..].ToLower();
    }

    private static string[] SplitWords(string text)
    {
        string[] words = text.Split(new[] { '-', '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return words;
    }

    public static string ToCamelCase(this string text)
    {
        string result = HandleBaseCases(text, false);
        if (result == null) return string.Empty;
        if (!string.IsNullOrEmpty(result)) return result;

        result = ToPascalCase(text);

        return result[..1].ToLower() + result[1..];
    }

    public static string ToSnakeCase(this string text)
    {
        string result = HandleBaseCases(text, false);
        if (result == null) return string.Empty;
        if (!string.IsNullOrEmpty(result)) return result;

        string[] words = SplitWords(text);

        return CasedStringWithSeparator(words, false, '_');
    }

    public static string ToCapsCase(this string text)
    {
        string result = HandleBaseCases(text, true);
        if (result == null) return string.Empty;
        if (!string.IsNullOrEmpty(result)) return result;

        string[] words = SplitWords(text);

        return CasedStringWithSeparator(words, true, '_');
    }

    public static string ToKebabCase(this string text)
    {
        string result = HandleBaseCases(text, false);
        if (result == null) return string.Empty;
        if (!string.IsNullOrEmpty(result)) return result;

        string[] words = SplitWords(text);

        return CasedStringWithSeparator(words, false, '-');
    }

    private static string HandleBaseCases(this string text, bool isUpper)
    {
        if (string.IsNullOrEmpty(text)) return null;
        return text.Length == 1 ? text.ConvertCase(isUpper) : string.Empty;
    }
    private static string ConvertCase(this string word, bool isUpper)
    {
        return isUpper ? word.ToUpper() : word.ToLower();
    }

    private static string CasedStringWithSeparator(IReadOnlyList<string> words, bool isUpper, char separator)
    {
        if (words.Count == 1) return words[0].ConvertCase(isUpper);

        StringBuilder sb = new();

        for (int i = 0; i < words.Count - 1; i++)
            sb.Append(words[i].ConvertCase(isUpper) + separator);

        sb.Append(words[^1].ConvertCase(isUpper));
        return sb.ToString();
    }

    public static string Left(this string text, int length)
    {
        if (length <= 0) return null;
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text.Length < length ? text : text[..length];
    }
}