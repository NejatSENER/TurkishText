using System.Globalization;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using TurkishText;

BenchmarkSwitcher.FromAssembly(typeof(TextBenchmarks).Assembly).Run(args);

[MemoryDiagnoser]
public class TextBenchmarks
{
    private static readonly CultureInfo Tr = new("tr-TR");
    private const string Text = "ŞİŞLİ'DE SATILIK DAİRE, ÇAĞLAYAN ADLİYESİ YAKINI, IĞDIR'LI EV SAHİBİNDEN";

    [Benchmark(Baseline = true)]
    public string ToLower_with_tr_culture() => Text.ToLower(Tr);

    [Benchmark]
    public string Turkish_ToLower() => Turkish.ToLower(Text);

    [Benchmark]
    public string Fold_with_normalize_FormD() => FoldWithNormalize(Text);

    [Benchmark]
    public string Turkish_Fold() => Turkish.Fold(Text);

    [Benchmark]
    public string Turkish_Slugify() => Turkish.Slugify(Text);

    private static string FoldWithNormalize(string value)
    {
        var decomposed = value.ToLower(Tr).Replace('ı', 'i').Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(decomposed.Length);
        foreach (var c in decomposed)
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) sb.Append(c);
        return sb.ToString();
    }
}
