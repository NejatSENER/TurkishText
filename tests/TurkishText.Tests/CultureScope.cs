using System.Globalization;

namespace TurkishText.Tests;

internal sealed class CultureScope : IDisposable
{
    private readonly CultureInfo _culture = CultureInfo.CurrentCulture;
    private readonly CultureInfo _uiCulture = CultureInfo.CurrentUICulture;

    public CultureScope(string name)
    {
        var culture = CultureInfo.InvariantCulture;
        if (name.Length > 0)
        {
            try { culture = new CultureInfo(name); }
            catch (CultureNotFoundException) { }
        }
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    public void Dispose()
    {
        CultureInfo.CurrentCulture = _culture;
        CultureInfo.CurrentUICulture = _uiCulture;
    }

    public static TheoryData<string> Cultures => new() { "", "en-US", "tr-TR", "az-Latn-AZ", "de-DE" };
}
