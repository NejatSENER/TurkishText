using System;
using System.Text;

namespace TurkishText;

public static partial class Turkish
{
    /// <summary>
    /// Builds a search key: Turkish lower-casing, then accents removed.
    /// <c>"Şişli"</c>, <c>"ŞİŞLİ"</c> and <c>"sisli"</c> all become <c>"sisli"</c>.
    /// Combining marks (for example a decomposed <c>"İ"</c>) are dropped.
    /// </summary>
    public static string Fold(string value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        var sb = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            var lower = ToLower(c);
            if (IsCombiningMark(lower)) continue;
            switch (lower)
            {
                case 'ß': sb.Append("ss"); break;
                case 'æ': sb.Append("ae"); break;
                case 'œ': sb.Append("oe"); break;
                default: sb.Append(FoldChar(lower)); break;
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Returns <c>true</c> when <paramref name="query"/> appears in <paramref name="text"/>,
    /// ignoring case and accents. <c>"sisli bel"</c> matches <c>"ŞİŞLİ Belediyesi"</c>.
    /// </summary>
    public static bool ContainsIgnoringCaseAndAccents(string text, string query)
    {
        if (text is null) throw new ArgumentNullException(nameof(text));
        if (query is null) throw new ArgumentNullException(nameof(query));
        return Fold(text).IndexOf(Fold(query), StringComparison.Ordinal) >= 0;
    }

    private static bool IsCombiningMark(char c) => c >= '̀' && c <= 'ͯ';

    private static char FoldChar(char c) => c switch
    {
        'ç' => 'c',
        'ğ' => 'g',
        'ı' => 'i',
        'ö' => 'o',
        'ş' => 's',
        'ü' => 'u',
        'â' or 'à' or 'á' or 'ä' or 'ã' or 'å' => 'a',
        'é' or 'è' or 'ê' or 'ë' => 'e',
        'î' or 'ì' or 'í' or 'ï' => 'i',
        'ô' or 'ò' or 'ó' or 'õ' or 'ø' => 'o',
        'û' or 'ù' or 'ú' => 'u',
        'ñ' => 'n',
        'ý' or 'ÿ' => 'y',
        _ => c,
    };
}
