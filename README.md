# Philiprehberger.PasswordStrength

[![CI](https://github.com/philiprehberger/dotnet-password-strength/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-password-strength/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.PasswordStrength.svg)](https://www.nuget.org/packages/Philiprehberger.PasswordStrength)
[![GitHub release](https://img.shields.io/github/v/release/philiprehberger/dotnet-password-strength)](https://github.com/philiprehberger/dotnet-password-strength/releases)
[![Last updated](https://img.shields.io/github/last-commit/philiprehberger/dotnet-password-strength)](https://github.com/philiprehberger/dotnet-password-strength/commits/main)
[![License](https://img.shields.io/github/license/philiprehberger/dotnet-password-strength)](LICENSE)
[![Bug Reports](https://img.shields.io/github/issues/philiprehberger/dotnet-password-strength/bug)](https://github.com/philiprehberger/dotnet-password-strength/issues?q=is%3Aissue+is%3Aopen+label%3Abug)
[![Feature Requests](https://img.shields.io/github/issues/philiprehberger/dotnet-password-strength/enhancement)](https://github.com/philiprehberger/dotnet-password-strength/issues?q=is%3Aissue+is%3Aopen+label%3Aenhancement)
[![Sponsor](https://img.shields.io/badge/sponsor-GitHub%20Sponsors-ec6cb9)](https://github.com/sponsors/philiprehberger)

Password strength evaluation with entropy calculation, common password detection, and pattern analysis.

## Installation

```bash
dotnet add package Philiprehberger.PasswordStrength
```

## Usage

```csharp
using Philiprehberger.PasswordStrength;

var score = Password.Evaluate("MyP@ssw0rd!");
Console.WriteLine(score.Score);     // 3 (0=very weak, 4=very strong)
Console.WriteLine(score.Entropy);   // 52.4 (bits)
Console.WriteLine(score.CrackTime); // "centuries"
foreach (var tip in score.Feedback)
    Console.WriteLine(tip);
```

### Context-Aware Checking

```csharp
using Philiprehberger.PasswordStrength;

var context = new PasswordContext
{
    UserName = "johndoe",
    Email = "john@example.com",
    FullName = "John Doe",
    DateOfBirth = new DateTime(1990, 5, 15)
};

var result = Password.Evaluate("johndoe1990!", context);
// Penalizes passwords containing username, email, name, or birth date
foreach (var tip in result.Feedback)
    Console.WriteLine(tip);
// "Password contains your username."
// "Password contains part of your email address."
// "Password contains your name ("john")."
// "Password contains your date of birth."
```

### Keyboard Pattern Detection

```csharp
using Philiprehberger.PasswordStrength;

var patterns = KeyboardPatternDetector.Detect("qwerty123!");
foreach (var pattern in patterns)
    Console.WriteLine(pattern);
// common keyboard pattern "qwerty"
// numeric sequence "1234" (if present)

var result = Password.Evaluate("asdfgh!!");
// Score is lowered due to keyboard pattern detection
```

### Passphrase Evaluation

```csharp
using Philiprehberger.PasswordStrength;

var result = Password.Evaluate("correct horse battery staple");
Console.WriteLine(result.Score); // 3 (four unique words scores highly)

// Passphrases are detected automatically when input has 3+ words
// separated by spaces, hyphens, or dots
var hyphenated = Password.Evaluate("solar-panel-winter-frost-glow");
Console.WriteLine(hyphenated.Score); // 4
```

### Structured Feedback

```csharp
using Philiprehberger.PasswordStrength;

var result = Password.Evaluate("qwerty");
foreach (var item in result.StructuredFeedback)
{
    Console.WriteLine($"[{item.Category}] {item.Message}");
}
// [CommonPassword] This is a commonly used password.
// [KeyboardPattern] Password contains a common keyboard pattern "qwerty".
// [TooShort] Use at least 8 characters.
// [LowDiversity] Consider adding uppercase letters.
// [LowDiversity] Consider adding numbers.
// [LowDiversity] Consider adding special characters.
```

### Common Password Detection and Policy Validation

```csharp
using Philiprehberger.PasswordStrength;

bool common = Password.IsCommon("password123"); // true
bool unique = Password.IsCommon("xK9#mP2!vL");  // false

var policy = new PasswordPolicy(MinLength: 12, MinScore: 3, RejectCommon: true);
bool valid = Password.MeetsPolicy("MyStr0ng!Pass", policy); // true
bool weak  = Password.MeetsPolicy("password", policy);      // false
```

## API

### `Password`

| Method | Description |
|--------|-------------|
| `Evaluate(string password)` | Evaluate strength and return a `PasswordScore` |
| `Evaluate(string password, PasswordContext? context)` | Evaluate strength with user context awareness |
| `IsCommon(string password)` | Check if the password is in the top 1000 common passwords |
| `MeetsPolicy(string password, PasswordPolicy policy)` | Check if the password satisfies the given policy |

### `PasswordScore`

| Member | Type | Description |
|--------|------|-------------|
| `Score` | `int` | Strength score from 0 (very weak) to 4 (very strong) |
| `Entropy` | `double` | Shannon entropy in bits |
| `CrackTime` | `string` | Human-readable crack time estimate |
| `Feedback` | `string[]` | Array of improvement suggestions |
| `StructuredFeedback` | `List<FeedbackItem>` | Categorized feedback with actionable messages |

### `FeedbackItem`

| Member | Type | Description |
|--------|------|-------------|
| `Category` | `FeedbackCategory` | Category of the weakness detected |
| `Message` | `string` | Human-readable, actionable suggestion |

### `FeedbackCategory`

| Value | Description |
|-------|-------------|
| `CommonPassword` | Matches a commonly used password |
| `KeyboardPattern` | Contains a keyboard pattern like "qwerty" |
| `TooShort` | Password is too short |
| `LowDiversity` | Missing uppercase, digits, or symbols |
| `ContextMatch` | Contains user context (username, email, name) |
| `SequentialChars` | Contains sequential characters |
| `RepeatedChars` | Contains repeated characters |
| `CommonSubstitution` | Uses common character substitutions |
| `CouldBeLonger` | Password could benefit from being longer |
| `FewWords` | Passphrase has too few words |

### `PasswordContext`

| Property | Type | Description |
|----------|------|-------------|
| `UserName` | `string?` | User's username |
| `Email` | `string?` | User's email address |
| `FullName` | `string?` | User's full name |
| `DateOfBirth` | `DateTime?` | User's date of birth |

### `KeyboardPatternDetector`

| Method | Description |
|--------|-------------|
| `Detect(string password)` | Detect keyboard patterns of 4+ adjacent keys |

### `PassphraseEvaluator`

| Method | Description |
|--------|-------------|
| `IsPassphrase(string input)` | Check if input has 3+ words separated by spaces, hyphens, or dots |
| `Evaluate(string passphrase)` | Evaluate passphrase using word count and vocabulary diversity |

### `EntropyCalculator`

| Method | Description |
|--------|-------------|
| `Calculate(string password)` | Calculate Shannon entropy in bits |
| `DetectPatterns(string password)` | Detect weakness patterns (sequential, repeated, keyboard walks, substitutions) |

### `PasswordPolicy`

| Member | Type | Default | Description |
|--------|------|---------|-------------|
| `MinLength` | `int` | `8` | Minimum required length |
| `MinScore` | `int` | `2` | Minimum required score 0-4 |
| `RejectCommon` | `bool` | `true` | Reject passwords in the common list |

## Development

```bash
dotnet build src/Philiprehberger.PasswordStrength.csproj --configuration Release
```

## Support

If you find this package useful, consider giving it a star on GitHub — it helps motivate continued maintenance and development.

[![LinkedIn](https://img.shields.io/badge/Philip%20Rehberger-LinkedIn-0A66C2?logo=linkedin)](https://www.linkedin.com/in/philiprehberger)
[![More packages](https://img.shields.io/badge/more-open%20source%20packages-blue)](https://philiprehberger.com/open-source-packages)

## License

[MIT](LICENSE)
