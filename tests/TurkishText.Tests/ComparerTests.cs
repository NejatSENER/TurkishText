namespace TurkishText.Tests;

public class ComparerTests
{
    [Fact]
    public void Ignore_case_follows_turkish_rules()
    {
        var comparer = TurkishEqualityComparer.IgnoreCase;
        Assert.True(comparer.Equals("İSTANBUL", "istanbul"));
        Assert.False(comparer.Equals("ISTANBUL", "istanbul"));
        Assert.True(comparer.Equals("ISPARTA", "ısparta"));
        Assert.Equal(comparer.GetHashCode("İSTANBUL"), comparer.GetHashCode("istanbul"));
    }

    [Fact]
    public void Ignore_case_and_accents_finds_ascii_spellings()
    {
        var districts = new HashSet<string>(TurkishEqualityComparer.IgnoreCaseAndAccents) { "Şişli", "Üsküdar" };
        Assert.Contains("SISLI", districts);
        Assert.Contains("uskudar", districts);
        Assert.DoesNotContain("Beşiktaş", districts);
    }

    [Fact]
    public void Works_with_dictionary_lookups()
    {
        var stock = new Dictionary<string, int>(TurkishEqualityComparer.IgnoreCaseAndAccents) { ["Çikolata"] = 3 };
        Assert.Equal(3, stock["CIKOLATA"]);
    }

    [Fact]
    public void Nulls_are_handled()
    {
        var comparer = TurkishEqualityComparer.IgnoreCase;
        Assert.True(comparer.Equals(null, null));
        Assert.False(comparer.Equals("a", null));
        Assert.False(comparer.Equals(null, "a"));
        Assert.Equal(0, comparer.GetHashCode(null));
    }
}
