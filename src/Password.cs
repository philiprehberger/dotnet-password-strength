namespace Philiprehberger.PasswordStrength;

/// <summary>
/// Provides static methods for evaluating password strength, detecting common passwords,
/// and validating against configurable policies.
/// </summary>
public static class Password
{
    /// <summary>
    /// Evaluates the strength of the specified password.
    /// </summary>
    /// <param name="password">The password to evaluate.</param>
    /// <returns>A <see cref="PasswordScore"/> containing the score, entropy, crack time estimate, and feedback.</returns>
    public static PasswordScore Evaluate(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        if (password.Length == 0)
        {
            return new PasswordScore(0, 0.0, "instant", ["Password is empty."]);
        }

        var feedback = new List<string>();
        double entropy = EntropyCalculator.Calculate(password);
        var patterns = EntropyCalculator.DetectPatterns(password);

        // Penalize entropy for detected patterns
        double effectiveEntropy = entropy;
        if (patterns.Length > 0)
        {
            effectiveEntropy *= Math.Max(0.3, 1.0 - (patterns.Length * 0.15));
        }

        // Penalize for being a common password
        if (IsCommon(password))
        {
            effectiveEntropy = Math.Min(effectiveEntropy, 5.0);
            feedback.Add("This is a commonly used password.");
        }

        // Length feedback
        if (password.Length < 8)
        {
            feedback.Add("Use at least 8 characters.");
        }
        else if (password.Length < 12)
        {
            feedback.Add("Consider using 12 or more characters.");
        }

        // Character diversity feedback
        bool hasLower = password.Any(char.IsLower);
        bool hasUpper = password.Any(char.IsUpper);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSymbol = password.Any(c => !char.IsLetterOrDigit(c));

        int charTypes = (hasLower ? 1 : 0) + (hasUpper ? 1 : 0) + (hasDigit ? 1 : 0) + (hasSymbol ? 1 : 0);

        if (!hasUpper)
        {
            feedback.Add("Add uppercase letters.");
        }

        if (!hasDigit)
        {
            feedback.Add("Add numbers.");
        }

        if (!hasSymbol)
        {
            feedback.Add("Add special characters.");
        }

        // Pattern feedback
        foreach (var pattern in patterns)
        {
            feedback.Add($"Detected pattern: {pattern}.");
        }

        // Score based on effective entropy and character diversity
        int score;
        if (effectiveEntropy < 20 || password.Length < 6)
        {
            score = 0;
        }
        else if (effectiveEntropy < 35 || charTypes < 2)
        {
            score = 1;
        }
        else if (effectiveEntropy < 50 || charTypes < 3)
        {
            score = 2;
        }
        else if (effectiveEntropy < 70)
        {
            score = 3;
        }
        else
        {
            score = 4;
        }

        string crackTime = EstimateCrackTime(effectiveEntropy);

        return new PasswordScore(score, Math.Round(entropy, 1), crackTime, feedback.ToArray());
    }

    /// <summary>
    /// Determines whether the specified password is in the list of top 1000 common passwords.
    /// </summary>
    /// <param name="password">The password to check.</param>
    /// <returns><c>true</c> if the password is common; otherwise, <c>false</c>.</returns>
    public static bool IsCommon(string password)
    {
        ArgumentNullException.ThrowIfNull(password);
        return CommonPasswords.Contains(password);
    }

    /// <summary>
    /// Determines whether the specified password satisfies the given policy.
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <param name="policy">The policy to validate against.</param>
    /// <returns><c>true</c> if the password meets all policy requirements; otherwise, <c>false</c>.</returns>
    public static bool MeetsPolicy(string password, PasswordPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(policy);

        if (password.Length < policy.MinLength)
        {
            return false;
        }

        if (policy.RejectCommon && IsCommon(password))
        {
            return false;
        }

        var score = Evaluate(password);
        return score.Score >= policy.MinScore;
    }

    private static string EstimateCrackTime(double entropy)
    {
        // Assume 10 billion guesses per second (modern GPU cluster)
        const double guessesPerSecond = 10_000_000_000.0;
        double combinations = Math.Pow(2, entropy);
        double seconds = combinations / guessesPerSecond / 2; // average case

        return seconds switch
        {
            < 1 => "instant",
            < 60 => "seconds",
            < 3600 => "minutes",
            < 86400 => "hours",
            < 2592000 => "days",
            < 31536000 => "months",
            < 3153600000 => "years",
            _ => "centuries"
        };
    }
}
