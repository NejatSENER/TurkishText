using System;

namespace TurkishText;

/// <summary>
/// Turkish text helpers. Every method gives the same result whatever the current culture is,
/// and also works when the app runs with invariant globalization (no ICU).
/// </summary>
public static partial class Turkish
{
    /// <summary>Lower-cases a character with Turkish rules: <c>I</c> becomes <c>ı</c>, <c>İ</c> becomes <c>i</c>.</summary>
    public static char ToLower(char c) => c switch
    {
        'I' => 'ı',
        'İ' => 'i',
        _ => char.ToLowerInvariant(c),
    };

    /// <summary>Upper-cases a character with Turkish rules: <c>i</c> becomes <c>İ</c>, <c>ı</c> becomes <c>I</c>.</summary>
    public static char ToUpper(char c) => c switch
    {
        'i' => 'İ',
        'ı' => 'I',
        _ => char.ToUpperInvariant(c),
    };

    /// <summary>Lower-cases text with Turkish rules. <c>"DİYARBAKIR"</c> becomes <c>"diyarbakır"</c>.</summary>
    public static string ToLower(string value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
#if NET8_0_OR_GREATER
        return string.Create(value.Length, value, static (span, source) =>
        {
            for (var i = 0; i < span.Length; i++) span[i] = ToLower(source[i]);
        });
#else
        var chars = new char[value.Length];
        for (var i = 0; i < value.Length; i++) chars[i] = ToLower(value[i]);
        return new string(chars);
#endif
    }

    /// <summary>Upper-cases text with Turkish rules. <c>"istanbul"</c> becomes <c>"İSTANBUL"</c>.</summary>
    public static string ToUpper(string value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
#if NET8_0_OR_GREATER
        return string.Create(value.Length, value, static (span, source) =>
        {
            for (var i = 0; i < span.Length; i++) span[i] = ToUpper(source[i]);
        });
#else
        var chars = new char[value.Length];
        for (var i = 0; i < value.Length; i++) chars[i] = ToUpper(value[i]);
        return new string(chars);
#endif
    }

    /// <summary>
    /// Capitalizes the first letter of every word and lower-cases the rest.
    /// Words start after white space or a hyphen, not after an apostrophe:
    /// <c>"ISTANBUL'DA İLK GÜN"</c> becomes <c>"Istanbul'da İlk Gün"</c>.
    /// </summary>
    public static string ToTitleCase(string value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        var chars = new char[value.Length];
        var startOfWord = true;
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            chars[i] = startOfWord ? ToUpper(c) : ToLower(c);
            startOfWord = char.IsWhiteSpace(c) || c == '-';
        }
        return new string(chars);
    }
}
