using System;
using System.Text;

namespace TurkishText;

public static partial class Turkish
{
    /// <summary>
    /// Makes a URL slug: <c>"Kadın Çantası &amp; Cüzdan"</c> becomes <c>"kadin-cantasi-cuzdan"</c>.
    /// Apostrophes are dropped so suffixes stay attached: <c>"İstanbul'da"</c> becomes <c>"istanbulda"</c>.
    /// </summary>
    /// <param name="value">Text to convert.</param>
    /// <param name="maxLength">Maximum length, 0 for no limit. Cuts at the last hyphen that fits, or mid-word when there is none.</param>
    public static string Slugify(string value, int maxLength = 0)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        if (maxLength < 0) throw new ArgumentOutOfRangeException(nameof(maxLength), "Must be 0 or greater.");

        var folded = Fold(value);
        var sb = new StringBuilder(folded.Length);
        var pendingHyphen = false;
        foreach (var c in folded)
        {
            if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
            {
                if (pendingHyphen && sb.Length > 0) sb.Append('-');
                pendingHyphen = false;
                sb.Append(c);
            }
            else if (c != '\'' && c != '’')
            {
                pendingHyphen = true;
            }
        }

        var slug = sb.ToString();
        if (maxLength == 0 || slug.Length <= maxLength) return slug;

        var cut = slug.LastIndexOf('-', maxLength);
        return cut > 0 ? slug.Substring(0, cut) : slug.Substring(0, maxLength);
    }
}
