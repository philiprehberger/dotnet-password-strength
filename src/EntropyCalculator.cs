namespace Philiprehberger.PasswordStrength;

/// <summary>
/// Calculates Shannon entropy and detects weakness patterns in passwords.
/// </summary>
public static class EntropyCalculator
{
    private static readonly string[] KeyboardRows =
    [
        "qwertyuiop",
        "asdfghjkl",
        "zxcvbnm",
        "1234567890"
    ];

    private static readonly Dictionary<char, char> CommonSubstitutions = new()
    {
        ['@'] = 'a',
        ['4'] = 'a',
        ['3'] = 'e',
        ['1'] = 'i',
        ['!'] = 'i',
        ['0'] = 'o',
        ['$'] = 's',
        ['5'] = 's',
        ['7'] = 't',
        ['+'] = 't',
        ['2'] = 'z',
        ['8'] = 'b',
        ['6'] = 'g',
        ['9'] = 'g'
    };

    /// <summary>
    /// Calculates the Shannon entropy of the specified password in bits.
    /// </summary>
    /// <param name="password">The password to analyze.</param>
    /// <returns>The entropy in bits.</returns>
    public static double Calculate(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        if (password.Length == 0)
        {
            return 0.0;
        }

        // Calculate pool size based on character classes present
        int poolSize = 0;

        if (password.Any(char.IsLower))
        {
            poolSize += 26;
        }

        if (password.Any(char.IsUpper))
        {
            poolSize += 26;
        }

        if (password.Any(char.IsDigit))
        {
            poolSize += 10;
        }

        if (password.Any(c => !char.IsLetterOrDigit(c)))
        {
            poolSize += 33;
        }

        if (poolSize == 0)
        {
            poolSize = 26;
        }

        // Entropy = length * log2(pool size)
        return password.Length * Math.Log2(poolSize);
    }

    /// <summary>
    /// Detects weakness patterns in the specified password.
    /// Checks for sequential characters, repeated characters, keyboard walks, and common substitutions.
    /// </summary>
    /// <param name="password">The password to analyze.</param>
    /// <returns>An array of detected pattern descriptions.</returns>
    public static string[] DetectPatterns(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        if (password.Length < 3)
        {
            return [];
        }

        var patterns = new List<string>();

        if (HasSequentialChars(password))
        {
            patterns.Add("sequential characters");
        }

        if (HasRepeatedChars(password))
        {
            patterns.Add("repeated characters");
        }

        if (HasKeyboardWalk(password))
        {
            patterns.Add("keyboard walk");
        }

        if (HasCommonSubstitutions(password))
        {
            patterns.Add("common substitutions");
        }

        return patterns.ToArray();
    }

    private static bool HasSequentialChars(string password)
    {
        var lower = password.ToLowerInvariant();

        for (int i = 0; i < lower.Length - 2; i++)
        {
            char a = lower[i];
            char b = lower[i + 1];
            char c = lower[i + 2];

            // Ascending sequence (abc, 123)
            if (b == a + 1 && c == b + 1)
            {
                return true;
            }

            // Descending sequence (cba, 321)
            if (b == a - 1 && c == b - 1)
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasRepeatedChars(string password)
    {
        for (int i = 0; i < password.Length - 2; i++)
        {
            if (password[i] == password[i + 1] && password[i + 1] == password[i + 2])
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasKeyboardWalk(string password)
    {
        var lower = password.ToLowerInvariant();

        foreach (var row in KeyboardRows)
        {
            for (int windowSize = 4; windowSize <= row.Length; windowSize++)
            {
                for (int start = 0; start <= row.Length - windowSize; start++)
                {
                    var segment = row.Substring(start, windowSize);
                    if (lower.Contains(segment, StringComparison.Ordinal))
                    {
                        return true;
                    }

                    // Reverse keyboard walk
                    var reversed = new string(segment.Reverse().ToArray());
                    if (lower.Contains(reversed, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private static bool HasCommonSubstitutions(string password)
    {
        int substitutionCount = 0;

        foreach (char c in password)
        {
            if (CommonSubstitutions.ContainsKey(c))
            {
                substitutionCount++;
            }
        }

        // Consider it a pattern if more than 30% of characters are substitutions
        return password.Length >= 4 && substitutionCount >= password.Length * 0.3;
    }
}
