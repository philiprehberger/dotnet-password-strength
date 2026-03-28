namespace Philiprehberger.PasswordStrength;

/// <summary>
/// Detects common keyboard patterns in passwords based on QWERTY layout adjacency.
/// </summary>
public static class KeyboardPatternDetector
{
    private static readonly string[] HorizontalRows =
    [
        "qwertyuiop",
        "asdfghjkl",
        "zxcvbnm",
        "1234567890"
    ];

    private static readonly string[] VerticalColumns =
    [
        "qaz",
        "wsx",
        "edc",
        "rfv",
        "tgb",
        "yhn",
        "ujm",
        "1qa",
        "2ws",
        "3ed",
        "4rf",
        "5tg",
        "6yh",
        "7uj",
        "8ik",
        "9ol",
        "0pl"
    ];

    private static readonly string[] CommonPatterns =
    [
        "qwerty",
        "asdfgh",
        "zxcvbn",
        "qazwsx",
        "qweasd",
        "asdzxc",
        "1qaz2wsx",
        "qweasdzxc"
    ];

    /// <summary>
    /// Detects keyboard patterns of 4 or more adjacent keys in the specified password.
    /// </summary>
    /// <param name="password">The password to analyze.</param>
    /// <returns>A list of detected keyboard pattern descriptions.</returns>
    public static List<string> Detect(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        if (password.Length < 4)
        {
            return [];
        }

        var lower = password.ToLowerInvariant();
        var results = new List<string>();

        // Check for well-known common patterns
        foreach (var pattern in CommonPatterns)
        {
            if (lower.Contains(pattern, StringComparison.Ordinal))
            {
                results.Add($"common keyboard pattern \"{pattern}\"");
            }
        }

        // Check for horizontal row sequences (4+ adjacent keys)
        foreach (var row in HorizontalRows)
        {
            DetectRowSequences(lower, row, results, "horizontal keyboard row");
        }

        // Check for vertical column sequences (3+ adjacent keys in a column)
        foreach (var col in VerticalColumns)
        {
            if (col.Length >= 3 && lower.Contains(col, StringComparison.Ordinal))
            {
                if (!results.Exists(r => r.Contains(col, StringComparison.Ordinal)))
                {
                    results.Add($"vertical keyboard pattern \"{col}\"");
                }
            }

            // Reverse column
            var reversed = new string(col.Reverse().ToArray());
            if (reversed.Length >= 3 && lower.Contains(reversed, StringComparison.Ordinal))
            {
                if (!results.Exists(r => r.Contains(reversed, StringComparison.Ordinal)))
                {
                    results.Add($"vertical keyboard pattern \"{reversed}\"");
                }
            }
        }

        // Check for numeric sequences
        DetectNumericSequences(lower, results);

        return results;
    }

    private static void DetectRowSequences(string password, string row, List<string> results, string label)
    {
        for (int windowSize = 4; windowSize <= row.Length; windowSize++)
        {
            for (int start = 0; start <= row.Length - windowSize; start++)
            {
                var segment = row.Substring(start, windowSize);

                if (password.Contains(segment, StringComparison.Ordinal))
                {
                    if (!results.Exists(r => r.Contains(segment, StringComparison.Ordinal)))
                    {
                        results.Add($"{label} \"{segment}\"");
                    }
                }

                var reversed = new string(segment.Reverse().ToArray());
                if (password.Contains(reversed, StringComparison.Ordinal))
                {
                    if (!results.Exists(r => r.Contains(reversed, StringComparison.Ordinal)))
                    {
                        results.Add($"{label} \"{reversed}\"");
                    }
                }
            }
        }
    }

    private static void DetectNumericSequences(string password, List<string> results)
    {
        const string digits = "1234567890";

        for (int windowSize = 4; windowSize <= digits.Length; windowSize++)
        {
            for (int start = 0; start <= digits.Length - windowSize; start++)
            {
                var segment = digits.Substring(start, windowSize);

                if (password.Contains(segment, StringComparison.Ordinal))
                {
                    if (!results.Exists(r => r.Contains(segment, StringComparison.Ordinal)))
                    {
                        results.Add($"numeric sequence \"{segment}\"");
                    }
                }

                var reversed = new string(segment.Reverse().ToArray());
                if (password.Contains(reversed, StringComparison.Ordinal))
                {
                    if (!results.Exists(r => r.Contains(reversed, StringComparison.Ordinal)))
                    {
                        results.Add($"numeric sequence \"{reversed}\"");
                    }
                }
            }
        }
    }
}
