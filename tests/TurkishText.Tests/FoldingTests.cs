namespace TurkishText.Tests;

public class FoldingTests
{
    [Theory]
    [InlineData("Şişli", "sisli")]
    [InlineData("ŞİŞLİ", "sisli")]
    [InlineData("IĞDIR", "igdir")]
    [InlineData("Çağlayan Gölü", "caglayan golu")]
    [InlineData("Üsküdar", "uskudar")]
    [InlineData("hâlâ", "hala")]
    [InlineData("Café Größe", "cafe grosse")]
    [InlineData("", "")]
    public void Fold_lowercases_and_removes_accents(string input, string expected) =>
        Assert.Equal(expected, Turkish.Fold(input));

    [Fact]
    public void Fold_drops_combining_marks_from_decomposed_text()
    {
        Assert.Equal("istanbul", Turkish.Fold("İstanbul"));
        Assert.Equal("sisli", Turkish.Fold("şişli"));
    }

    [Theory]
    [MemberData(nameof(CultureScope.Cultures), MemberType = typeof(CultureScope))]
    public void Fold_does_not_depend_on_current_culture(string culture)
    {
        using var _ = new CultureScope(culture);
        Assert.Equal("diyarbakir", Turkish.Fold("DİYARBAKIR"));
        Assert.Equal("isik", Turkish.Fold("IŞIK"));
    }

    [Theory]
    [InlineData("ŞİŞLİ Belediyesi", "sisli bel", true)]
    [InlineData("Kadıköy İskelesi", "KADIKOY", true)]
    [InlineData("Kadıköy", "beşiktaş", false)]
    [InlineData("anything", "", true)]
    public void Contains_ignores_case_and_accents(string text, string query, bool expected) =>
        Assert.Equal(expected, Turkish.ContainsIgnoringCaseAndAccents(text, query));

    [Fact]
    public void Null_throws()
    {
        Assert.Throws<ArgumentNullException>(() => Turkish.Fold(null!));
        Assert.Throws<ArgumentNullException>(() => Turkish.ContainsIgnoringCaseAndAccents(null!, "a"));
        Assert.Throws<ArgumentNullException>(() => Turkish.ContainsIgnoringCaseAndAccents("a", null!));
    }
}
