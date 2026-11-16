# TurkishText

[![CI](https://github.com/NejatSENER/TurkishText/actions/workflows/ci.yml/badge.svg)](https://github.com/NejatSENER/TurkishText/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/TurkishText.svg)](https://www.nuget.org/packages/TurkishText)

Small, dependency-free helpers for Turkish text in .NET:

- **Casing** that gets `İ/i` and `I/ı` right on every machine
- **Search folding**: `"sisli"` finds `"ŞİŞLİ"`
- **URL slugs**: `"Kadın Çantası & Cüzdan"` → `kadin-cantasi-cuzdan`
- **Numbers and amounts in words** for invoices: `1234.50m` → `bin iki yüz otuz dört Türk lirası elli kuruş`

Every method returns the same result whatever `CultureInfo.CurrentCulture` is, and keeps working when the app runs with invariant globalization (no ICU), which is the default in many Docker images.

```bash
dotnet add package TurkishText
```

Targets `netstandard2.0` and `net8.0`.

## Why

.NET casing depends on the current culture, so the same code behaves differently on a Turkish machine and on a server:

```csharp
"ISTANBUL".ToLower();                          // "istanbul" on en-US, "ıstanbul" on tr-TR
"istanbul".ToUpper(new CultureInfo("tr-TR"));  // throws CultureNotFoundException with invariant globalization
```

`ToLowerInvariant()` avoids the culture, but then Turkish text comes out wrong: `"DİYARBAKIR".ToLowerInvariant()` is `"dİyarbakir"` (the `İ` is left upper case and the last `I` becomes `i` instead of `ı`). TurkishText applies Turkish rules explicitly, without asking the OS.

## Usage

```csharp
using TurkishText;

Turkish.ToLower("DİYARBAKIR");                 // "diyarbakır"
Turkish.ToUpper("istanbul");                   // "İSTANBUL"
Turkish.ToTitleCase("İZMİR'DE IĞDIR GÜNÜ");    // "İzmir'de Iğdır Günü"

Turkish.Fold("ŞİŞLİ");                         // "sisli"
Turkish.ContainsIgnoringCaseAndAccents("Kadıköy İskelesi", "KADIKOY");  // true

Turkish.Slugify("İstanbul'da Kahvaltı");       // "istanbulda-kahvalti"
Turkish.Slugify("Kadın Çantası Cüzdan", maxLength: 15);  // "kadin-cantasi"

Turkish.NumberToWords(1_234_567);
// "bir milyon iki yüz otuz dört bin beş yüz altmış yedi"

Turkish.AmountToWords(1234.50m);
// "bin iki yüz otuz dört Türk lirası elli kuruş"
Turkish.AmountToWords(1234.50m, WordCasing.Title, compact: true);
// "BinİkiYüzOtuzDörtTürkLirasıElliKuruş"
Turkish.AmountToWords(1234.50m, WordCasing.Upper, currency: "TL", subunit: "Kr");
// "BİN İKİ YÜZ OTUZ DÖRT TL ELLİ KR"
```

### Comparers

```csharp
var districts = new HashSet<string>(TurkishEqualityComparer.IgnoreCaseAndAccents) { "Şişli", "Üsküdar" };
districts.Contains("SISLI");     // true

TurkishEqualityComparer.IgnoreCase.Equals("İSTANBUL", "istanbul");  // true
TurkishEqualityComparer.IgnoreCase.Equals("ISTANBUL", "istanbul");  // false: I is the capital of ı
```

The comparers are for equality (dictionaries, `HashSet`, `Distinct`, `GroupBy`), not for alphabetical sorting.

## API

| Method | What it does |
|---|---|
| `Turkish.ToLower` / `ToUpper` | Turkish casing for a `char` or `string` |
| `Turkish.ToTitleCase` | Capitalizes words after white space or `-`, never after an apostrophe |
| `Turkish.Fold` | Turkish lower case, then accents removed (`ç ğ ı ö ş ü â î û` and common Latin accents) |
| `Turkish.ContainsIgnoringCaseAndAccents` | Substring search on folded text |
| `Turkish.Slugify` | `a-z`, `0-9` and single hyphens; optional `maxLength` cuts at a word boundary |
| `Turkish.NumberToWords` | Any `long`, with `eksi` for negatives |
| `Turkish.AmountToWords` | Rounds to 2 decimals (away from zero), lira and kuruş, custom unit names |
| `TurkishEqualityComparer.IgnoreCase` / `IgnoreCaseAndAccents` | `IEqualityComparer<string?>` |

## Performance

BenchmarkDotNet, .NET 10, Apple M5, a 72-character Turkish sentence:

| Method | Mean | Allocated |
|---|---:|---:|
| `string.ToLower(new CultureInfo("tr-TR"))` | 114 ns | 168 B |
| `Turkish.ToLower` | 64 ns | 168 B |
| Folding with `Normalize(FormD)` + removing marks | 414 ns | 912 B |
| `Turkish.Fold` | 147 ns | 384 B |
| `Turkish.Slugify` | 211 ns | 760 B |

Run them with `dotnet run -c Release --project benchmarks/TurkishText.Benchmarks`.

## Tests

The test suite runs on Linux, Windows and macOS, under several current cultures (`tr-TR`, `az-Latn-AZ`, `en-US`, `de-DE`, invariant) and once more with `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1`.

```bash
dotnet test
```

## License

MIT
