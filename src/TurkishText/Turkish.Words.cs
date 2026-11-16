using System;
using System.Collections.Generic;
using System.Text;

namespace TurkishText;

/// <summary>How <see cref="Turkish.NumberToWords"/> and <see cref="Turkish.AmountToWords"/> case their output.</summary>
public enum WordCasing
{
    /// <summary>Normal Turkish spelling: <c>bin iki yüz Türk lirası</c>.</summary>
    AsWritten,
    /// <summary>All lower case: <c>bin iki yüz türk lirası</c>.</summary>
    Lower,
    /// <summary>First letter of every word capitalized, the rest kept: <c>Bin İki Yüz Türk Lirası</c>, <c>Bin TL</c>.</summary>
    Title,
    /// <summary>All upper case: <c>BİN İKİ YÜZ TÜRK LİRASI</c>.</summary>
    Upper,
}

public static partial class Turkish
{
    private static readonly string[] Ones = { "", "bir", "iki", "üç", "dört", "beş", "altı", "yedi", "sekiz", "dokuz" };
    private static readonly string[] Tens = { "", "on", "yirmi", "otuz", "kırk", "elli", "altmış", "yetmiş", "seksen", "doksan" };
    private static readonly string[] Scales = { "", "bin", "milyon", "milyar", "trilyon", "katrilyon", "kentilyon" };

    /// <summary>
    /// Writes a whole number in Turkish words: <c>1234</c> becomes <c>"bin iki yüz otuz dört"</c>.
    /// </summary>
    /// <param name="number">Any <see cref="long"/>, negatives start with <c>"eksi"</c>.</param>
    /// <param name="casing">Output casing.</param>
    /// <param name="compact">Join words without spaces: <c>"binikiyüzotuzdört"</c>.</param>
    public static string NumberToWords(long number, WordCasing casing = WordCasing.AsWritten, bool compact = false)
    {
        var words = new List<string>();
        if (number == 0)
        {
            words.Add("sıfır");
        }
        else
        {
            if (number < 0) words.Add("eksi");
            var magnitude = number < 0 ? (ulong)(-(number + 1)) + 1 : (ulong)number;
            AppendNumber(magnitude, words);
        }
        return Join(words, casing, compact);
    }

    /// <summary>
    /// Writes a money amount in Turkish words, rounded to two decimals:
    /// <c>1234.50m</c> becomes <c>"bin iki yüz otuz dört Türk lirası elli kuruş"</c>.
    /// Use it for the "yalnız ..." line on invoices and receipts.
    /// </summary>
    /// <param name="amount">The amount. Its whole part must fit in a <see cref="long"/>.</param>
    /// <param name="casing">Output casing.</param>
    /// <param name="compact">Join words without spaces: <c>"BinİkiYüzOtuzDörtTürkLirasıElliKuruş"</c> with <see cref="WordCasing.Title"/>.</param>
    /// <param name="currency">Main unit name, for example <c>"TL"</c>.</param>
    /// <param name="subunit">Sub-unit name, for example <c>"Kr"</c>.</param>
    public static string AmountToWords(
        decimal amount,
        WordCasing casing = WordCasing.AsWritten,
        bool compact = false,
        string currency = "Türk lirası",
        string subunit = "kuruş")
    {
        if (currency is null) throw new ArgumentNullException(nameof(currency));
        if (subunit is null) throw new ArgumentNullException(nameof(subunit));

        var rounded = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        var absolute = Math.Abs(rounded);
        var whole = decimal.Truncate(absolute);
        if (whole > long.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(amount), "The whole part must fit in a long.");

        var lira = (ulong)whole;
        var kurus = (ulong)((absolute - whole) * 100);

        var words = new List<string>();
        if (rounded < 0) words.Add("eksi");
        if (lira > 0 || kurus == 0)
        {
            if (lira == 0) words.Add("sıfır");
            else AppendNumber(lira, words);
            words.AddRange(SplitWords(currency));
        }
        if (kurus > 0)
        {
            AppendNumber(kurus, words);
            words.AddRange(SplitWords(subunit));
        }
        return Join(words, casing, compact);
    }

    private static void AppendNumber(ulong number, List<string> words)
    {
        var groups = new List<int>();
        while (number > 0)
        {
            groups.Add((int)(number % 1000));
            number /= 1000;
        }

        for (var scale = groups.Count - 1; scale >= 0; scale--)
        {
            var group = groups[scale];
            if (group == 0) continue;
            var isOneThousand = scale == 1 && group == 1;
            if (!isOneThousand) AppendHundreds(group, words);
            if (scale > 0) words.Add(Scales[scale]);
        }
    }

    private static void AppendHundreds(int group, List<string> words)
    {
        var hundreds = group / 100;
        var tens = group / 10 % 10;
        var ones = group % 10;
        if (hundreds > 0)
        {
            if (hundreds > 1) words.Add(Ones[hundreds]);
            words.Add("yüz");
        }
        if (tens > 0) words.Add(Tens[tens]);
        if (ones > 0) words.Add(Ones[ones]);
    }

    private static string[] SplitWords(string text) =>
        text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

    private static string Join(List<string> words, WordCasing casing, bool compact)
    {
        var sb = new StringBuilder();
        foreach (var word in words)
        {
            if (sb.Length > 0 && !compact) sb.Append(' ');
            sb.Append(casing switch
            {
                WordCasing.Lower => ToLower(word),
                WordCasing.Upper => ToUpper(word),
                WordCasing.Title => ToUpper(word[0]) + word.Substring(1),
                _ => word,
            });
        }
        return sb.ToString();
    }
}
