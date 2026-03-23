# Philiprehberger.PasswordStrength

[![CI](https://github.com/philiprehberger/dotnet-password-strength/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-password-strength/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.PasswordStrength.svg)](https://www.nuget.org/packages/Philiprehberger.PasswordStrength)
[![License](https://img.shields.io/github/license/philiprehberger/dotnet-password-strength)](LICENSE)

Password strength evaluation with entropy calculation, common password detection, and pattern analysis.

## Installation

```bash
dotnet add package Philiprehberger.PasswordStrength
```

## Usage

```csharp
using Philiprehberger.PasswordStrength;

// Evaluate password strength
var score = Password.Evaluate("MyP@ssw0rd!");
Console.WriteLine(score.Score);     // 3 (0=very weak, 4=very strong)
Console.WriteLine(score.Entropy);   // 52.4 (bits)
Console.WriteLine(score.CrackTime); // "centuries"
foreach (var tip in score.Feedback)
    Console.WriteLine(tip);

// Check against common passwords
bool common = Password.IsCommon("password123"); // true
bool unique = Password.IsCommon("xK9#mP2!vL");  // false

// Policy-based validation
var policy = new PasswordPolicy
{
    MinLength = 12,
    MinScore = 3,
    RejectCommon = true
};

bool valid = Password.MeetsPolicy("MyStr0ng!Pass", policy); // true
bool weak  = Password.MeetsPolicy("password", policy);      // false

// Direct entropy calculation
double entropy = EntropyCalculator.Calculate("hello");
var patterns = EntropyCalculator.DetectPatterns("qwerty123");
// ["keyboard walk", "sequential digits"]
```

## API

### `Password`

| Member | Description |
|--------|-------------|
| `Evaluate(string password)` | Evaluate strength and return a `PasswordScore` |
| `IsCommon(string password)` | Check if the password is in the top 1000 common passwords |
| `MeetsPolicy(string password, PasswordPolicy policy)` | Check if the password satisfies the given policy |

### `PasswordScore`

| Member | Description |
|--------|-------------|
| `Score` | Strength score from 0 (very weak) to 4 (very strong) |
| `Entropy` | Shannon entropy in bits |
| `CrackTime` | Human-readable crack time estimate |
| `Feedback` | Array of improvement suggestions |

### `EntropyCalculator`

| Member | Description |
|--------|-------------|
| `Calculate(string password)` | Calculate Shannon entropy in bits |
| `DetectPatterns(string password)` | Detect weakness patterns (sequential, repeated, keyboard walks, substitutions) |

### `PasswordPolicy`

| Member | Description |
|--------|-------------|
| `MinLength` | Minimum required length (default: 8) |
| `MinScore` | Minimum required score 0-4 (default: 2) |
| `RejectCommon` | Reject passwords in the common list (default: true) |

## Development

```bash
dotnet build src/Philiprehberger.PasswordStrength.csproj --configuration Release
```

## License

MIT
