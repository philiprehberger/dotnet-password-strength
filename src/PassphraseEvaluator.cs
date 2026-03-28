namespace Philiprehberger.PasswordStrength;

/// <summary>
/// Evaluates word-based passphrases using word count and vocabulary diversity
/// rather than character-level metrics.
/// </summary>
public static class PassphraseEvaluator
{
    private static readonly char[] WordSeparators = [' ', '-', '.'];

    /// <summary>
    /// Determines whether the input is a passphrase (3 or more words separated by spaces, hyphens, or dots).
    /// </summary>
    /// <param name="input">The input string to check.</param>
    /// <returns><c>true</c> if the input contains 3 or more separated words; otherwise, <c>false</c>.</returns>
    public static bool IsPassphrase(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        var words = SplitWords(input);
        return words.Length >= 3;
    }

    /// <summary>
    /// Evaluates a passphrase and returns a <see cref="PasswordScore"/> based on word count and vocabulary diversity.
    /// </summary>
    /// <param name="passphrase">The passphrase to evaluate.</param>
    /// <returns>A <see cref="PasswordScore"/> with score, entropy, crack time, and feedback.</returns>
    public static PasswordScore Evaluate(string passphrase)
    {
        ArgumentNullException.ThrowIfNull(passphrase);

        var words = SplitWords(passphrase);
        var feedback = new List<string>();
        var structuredFeedback = new List<FeedbackItem>();

        int wordCount = words.Length;
        int uniqueWords = words.Select(w => w.ToLowerInvariant()).Distinct().Count();

        // Estimate entropy: assume a dictionary of ~7776 words (like diceware)
        // Entropy per word = log2(7776) ~ 12.9 bits
        double entropyPerWord = Math.Log2(7776);
        double entropy = uniqueWords * entropyPerWord;

        // Penalize duplicate words
        if (uniqueWords < wordCount)
        {
            feedback.Add("Avoid repeating words in the passphrase.");
            structuredFeedback.Add(new FeedbackItem(FeedbackCategory.LowDiversity, "Avoid repeating words in the passphrase."));
            entropy *= (double)uniqueWords / wordCount;
        }

        // Score based on unique word count
        int score;
        if (uniqueWords < 3)
        {
            score = 1;
            feedback.Add("Use at least 3 unique words.");
            structuredFeedback.Add(new FeedbackItem(FeedbackCategory.FewWords, "Use at least 3 unique words."));
        }
        else if (uniqueWords == 3)
        {
            score = 2;
            feedback.Add("Consider adding a 4th word for stronger security.");
            structuredFeedback.Add(new FeedbackItem(FeedbackCategory.FewWords, "Consider adding a 4th word for stronger security."));
        }
        else if (uniqueWords == 4)
        {
            score = 3;
        }
        else
        {
            score = 4;
        }

        // Check for common passwords within individual words
        foreach (var word in words.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (CommonPasswords.Contains(word))
            {
                feedback.Add($"The word \"{word}\" is a commonly used password.");
                structuredFeedback.Add(new FeedbackItem(FeedbackCategory.CommonPassword, $"The word \"{word}\" is a commonly used password."));
            }
        }

        string crackTime = EstimatePassphraseCrackTime(entropy);

        return new PasswordScore(score, Math.Round(entropy, 1), crackTime, feedback.ToArray())
        {
            StructuredFeedback = structuredFeedback
        };
    }

    internal static string[] SplitWords(string input)
    {
        return input.Split(WordSeparators, StringSplitOptions.RemoveEmptyEntries);
    }

    private static string EstimatePassphraseCrackTime(double entropy)
    {
        const double guessesPerSecond = 10_000_000_000.0;
        double combinations = Math.Pow(2, entropy);
        double seconds = combinations / guessesPerSecond / 2;

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
