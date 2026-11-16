namespace TurkishText.Tests;

public class SlugTests
{
    [Theory]
    [InlineData("Kadın Çantası & Cüzdan", "kadin-cantasi-cuzdan")]
    [InlineData("  --Merhaba,  Dünya!-- ", "merhaba-dunya")]
    [InlineData("İstanbul'da Kahvaltı", "istanbulda-kahvalti")]
    [InlineData("Ağrı’dan Iğdır’a", "agridan-igdira")]
    [InlineData("iPhone 15 Pro (256 GB)", "iphone-15-pro-256-gb")]
    [InlineData("%100 Pamuk", "100-pamuk")]
    [InlineData("!!!", "")]
    [InlineData("", "")]
    public void Slugify_makes_lowercase_ascii_slugs(string input, string expected) =>
        Assert.Equal(expected, Turkish.Slugify(input));

    [Theory]
    [InlineData("Kadın Çantası Cüzdan", 15, "kadin-cantasi")]
    [InlineData("Kadın Çantası Cüzdan", 13, "kadin-cantasi")]
    [InlineData("Kadın Çantası Cüzdan", 100, "kadin-cantasi-cuzdan")]
    [InlineData("Kadın", 3, "kad")]
    public void Slugify_cuts_at_the_last_hyphen_that_fits(string input, int maxLength, string expected) =>
        Assert.Equal(expected, Turkish.Slugify(input, maxLength));

    [Fact]
    public void Invalid_arguments_throw()
    {
        Assert.Throws<ArgumentNullException>(() => Turkish.Slugify(null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => Turkish.Slugify("a", -1));
    }
}
