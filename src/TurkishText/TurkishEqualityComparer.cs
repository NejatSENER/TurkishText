using System;
using System.Collections.Generic;

namespace TurkishText;

/// <summary>
/// Equality comparers for dictionaries, hash sets and LINQ <c>Distinct</c>/<c>GroupBy</c>
/// that follow Turkish casing whatever the current culture is.
/// </summary>
public sealed class TurkishEqualityComparer : IEqualityComparer<string?>
{
    /// <summary>Ignores case with Turkish rules: <c>"İSTANBUL"</c> equals <c>"istanbul"</c>, <c>"ISTANBUL"</c> does not.</summary>
    public static TurkishEqualityComparer IgnoreCase { get; } = new(Turkish.ToLower);

    /// <summary>Ignores case and accents: <c>"Şişli"</c>, <c>"ŞİŞLİ"</c> and <c>"sisli"</c> are equal.</summary>
    public static TurkishEqualityComparer IgnoreCaseAndAccents { get; } = new(Turkish.Fold);

    private readonly Func<string, string> _key;

    private TurkishEqualityComparer(Func<string, string> key) => _key = key;

    /// <inheritdoc />
    public bool Equals(string? x, string? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return string.Equals(_key(x), _key(y), StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public int GetHashCode(string? obj) =>
        obj is null ? 0 : StringComparer.Ordinal.GetHashCode(_key(obj));
}
