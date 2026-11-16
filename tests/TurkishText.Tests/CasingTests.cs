namespace TurkishText.Tests;

public class CasingTests
{
    [Theory]
    [MemberData(nameof(CultureScope.Cultures), MemberType = typeof(CultureScope))]
    public void Results_do_not_depend_on_current_culture(string culture)
    {
        using var _ = new CultureScope(culture);

        Assert.Equal("diyarbakır", Turkish.ToLower("DİYARBAKIR"));
        Assert.Equal("İSTANBUL", Turkish.ToUpper("istanbul"));
        Assert.Equal("ILIK", Turkish.ToUpper("ılık"));
        Assert.Equal("İzmir'de Iğdır Günü", Turkish.ToTitleCase("İZMİR'DE IĞDIR GÜNÜ"));
    }

    [Fact]
    public void Dotless_and_dotted_i_round_trip()
    {
        const string text = "Işık İçin";
        Assert.Equal("ışık için", Turkish.ToLower(text));
        Assert.Equal("IŞIK İÇİN", Turkish.ToUpper(text));
        Assert.Equal("IŞIK İÇİN", Turkish.ToUpper(Turkish.ToLower(text)));
    }

    [Theory]
    [InlineData("istanbul'da güzel bir gün", "İstanbul'da Güzel Bir Gün")]
    [InlineData("ahmet-ali yılmaz", "Ahmet-Ali Yılmaz")]
    [InlineData("  çok   boşluk ", "  Çok   Boşluk ")]
    [InlineData("", "")]
    public void Title_case_starts_words_after_space_or_hyphen_only(string input, string expected) =>
        Assert.Equal(expected, Turkish.ToTitleCase(input));

    [Fact]
    public void Non_turkish_letters_use_invariant_casing()
    {
        Assert.Equal("straße école", Turkish.ToLower("STRAßE ÉCOLE"));
        Assert.Equal("ÉCOLE", Turkish.ToUpper("école"));
    }

    [Fact]
    public void Null_throws()
    {
        Assert.Throws<ArgumentNullException>(() => Turkish.ToLower(null!));
        Assert.Throws<ArgumentNullException>(() => Turkish.ToUpper(null!));
        Assert.Throws<ArgumentNullException>(() => Turkish.ToTitleCase(null!));
    }
}
