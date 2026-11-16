namespace TurkishText.Tests;

public class WordsTests
{
    [Theory]
    [InlineData(0, "sıfır")]
    [InlineData(1, "bir")]
    [InlineData(10, "on")]
    [InlineData(11, "on bir")]
    [InlineData(40, "kırk")]
    [InlineData(100, "yüz")]
    [InlineData(101, "yüz bir")]
    [InlineData(200, "iki yüz")]
    [InlineData(999, "dokuz yüz doksan dokuz")]
    [InlineData(1000, "bin")]
    [InlineData(1001, "bin bir")]
    [InlineData(2000, "iki bin")]
    [InlineData(11000, "on bir bin")]
    [InlineData(101000, "yüz bir bin")]
    [InlineData(1_000_000, "bir milyon")]
    [InlineData(1_001_000, "bir milyon bin")]
    [InlineData(1_234_567, "bir milyon iki yüz otuz dört bin beş yüz altmış yedi")]
    [InlineData(1_000_000_000, "bir milyar")]
    [InlineData(-5, "eksi beş")]
    public void Number_to_words(long number, string expected) =>
        Assert.Equal(expected, Turkish.NumberToWords(number));

    [Fact]
    public void Extremes_do_not_overflow()
    {
        Assert.Equal(
            "dokuz kentilyon iki yüz yirmi üç katrilyon üç yüz yetmiş iki trilyon otuz altı milyar sekiz yüz elli dört milyon yedi yüz yetmiş beş bin sekiz yüz yedi",
            Turkish.NumberToWords(long.MaxValue));
        Assert.StartsWith("eksi dokuz kentilyon", Turkish.NumberToWords(long.MinValue));
        Assert.EndsWith("sekiz yüz sekiz", Turkish.NumberToWords(long.MinValue));
    }

    [Theory]
    [InlineData(1234.50, "bin iki yüz otuz dört Türk lirası elli kuruş")]
    [InlineData(0.75, "yetmiş beş kuruş")]
    [InlineData(0, "sıfır Türk lirası")]
    [InlineData(1, "bir Türk lirası")]
    [InlineData(10.005, "on Türk lirası bir kuruş")]
    [InlineData(-2.5, "eksi iki Türk lirası elli kuruş")]
    [InlineData(1000000.01, "bir milyon Türk lirası bir kuruş")]
    public void Amount_to_words(double amount, string expected) =>
        Assert.Equal(expected, Turkish.AmountToWords((decimal)amount));

    [Fact]
    public void Amount_casing_and_compact_forms()
    {
        Assert.Equal("BİN İKİ YÜZ OTUZ DÖRT TÜRK LİRASI ELLİ KURUŞ", Turkish.AmountToWords(1234.50m, WordCasing.Upper));
        Assert.Equal("Bin İki Yüz Otuz Dört Türk Lirası Elli Kuruş", Turkish.AmountToWords(1234.50m, WordCasing.Title));
        Assert.Equal("bin iki yüz otuz dört türk lirası elli kuruş", Turkish.AmountToWords(1234.50m, WordCasing.Lower));
        Assert.Equal("BinİkiYüzOtuzDörtTürkLirasıElliKuruş", Turkish.AmountToWords(1234.50m, WordCasing.Title, compact: true));
        Assert.Equal("Bin İki Yüz Otuz Dört TL Elli Kr", Turkish.AmountToWords(1234.50m, WordCasing.Title, currency: "TL", subunit: "Kr"));
    }

    [Fact]
    public void Rounding_is_away_from_zero()
    {
        Assert.Equal("bir kuruş", Turkish.AmountToWords(0.005m));
        Assert.Equal("bir Türk lirası", Turkish.AmountToWords(0.995m));
    }

    [Theory]
    [MemberData(nameof(CultureScope.Cultures), MemberType = typeof(CultureScope))]
    public void Upper_case_output_does_not_depend_on_current_culture(string culture)
    {
        using var _ = new CultureScope(culture);
        Assert.Equal("İKİ BİN", Turkish.NumberToWords(2000, WordCasing.Upper));
    }

    [Fact]
    public void Too_large_amount_throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Turkish.AmountToWords(decimal.MaxValue));
}
