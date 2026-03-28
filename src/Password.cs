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
        return Evaluate(password, null);
    }

    /// <summary>
    /// Evaluates the strength of the specified password with user context awareness.
    /// Passwords containing context values (username, email, name, date of birth) are penalized.
    /// </summary>
    /// <param name="password">The password to evaluate.</param>
    /// <param name="context">Optional user context for context-aware checking.</param>
    /// <returns>A <see cref="PasswordScore"/> containing the score, entropy, crack time estimate, and feedback.</returns>
    public static PasswordScore Evaluate(string password, PasswordContext? context)
    {
        ArgumentNullException.ThrowIfNull(password);

        if (password.Length == 0)
        {
            return new PasswordScore(0, 0.0, "instant", ["Password is empty."])
            {
                StructuredFeedback = [new FeedbackItem(FeedbackCategory.TooShort, "Password is empty.")]
            };
        }

        // Check if input is a passphrase
        if (PassphraseEvaluator.IsPassphrase(password))
        {
            var passphraseResult = PassphraseEvaluator.Evaluate(password);

            // If context is provided, also check context matches for passphrases
            if (context is not null)
            {
                var contextFeedback = new List<string>(passphraseResult.Feedback);
                var contextStructured = new List<FeedbackItem>(passphraseResult.StructuredFeedback);
                int ctxPenalty = CheckContext(password, context, contextFeedback, contextStructured);
                int adjustedScore = Math.Max(0, passphraseResult.Score - ctxPenalty);

                return new PasswordScore(adjustedScore, passphraseResult.Entropy, passphraseResult.CrackTime, contextFeedback.ToArray())
                {
                    StructuredFeedback = contextStructured
                };
            }

            return passphraseResult;
        }

        var feedback = new List<string>();
        var structuredFeedback = new List<FeedbackItem>();
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
            structuredFeedback.Add(new FeedbackItem(FeedbackCategory.CommonPassword, "This is a commonly used password."));
        }

        // Keyboard pattern detection
        var keyboardPatterns = KeyboardPatternDetector.Detect(password);
        if (keyboardPatterns.Count > 0)
        {
            effectiveEntropy *= Math.Max(0.3, 1.0 - (keyboardPatterns.Count * 0.1));
            foreach (var kp in keyboardPatterns)
            {
                feedback.Add($"Password contains a {kp}.");
                structuredFeedback.Add(new FeedbackItem(FeedbackCategory.KeyboardPattern, $"Password contains a {kp}."));
            }
        }

        // Length feedback
        if (password.Length < 8)
        {
            feedback.Add("Use at least 8 characters.");
            structuredFeedback.Add(new FeedbackItem(FeedbackCategory.TooShort, "Use at least 8 characters."));
        }
        else if (password.Length < 12)
        {
            feedback.Add("Consider using 12 or more characters.");
            structuredFeedback.Add(new FeedbackItem(FeedbackCategory.CouldBeLonger, "Consider using 12 or more characters."));
        }

        // Character diversity feedback
        bool hasLower = password.Any(char.IsLower);
        bool hasUpper = password.Any(char.IsUpper);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSymbol = password.Any(c => !char.IsLetterOrDigit(c));

        int charTypes = (hasLower ? 1 : 0) + (hasUpper ? 1 : 0) + (hasDigit ? 1 : 0) + (hasSymbol ? 1 : 0);

        if (!hasUpper)
        {
            feedback.Add("Consider adding uppercase letters.");
            structuredFeedback.Add(new FeedbackItem(FeedbackCategory.LowDiversity, "Consider adding uppercase letters."));
        }

        if (!hasDigit)
        {
            feedback.Add("Consider adding numbers.");
            structuredFeedback.Add(new FeedbackItem(FeedbackCategory.LowDiversity, "Consider adding numbers."));
        }

        if (!hasSymbol)
        {
            feedback.Add("Consider adding special characters.");
            structuredFeedback.Add(new FeedbackItem(FeedbackCategory.LowDiversity, "Consider adding special characters."));
        }

        // Pattern feedback
        foreach (var pattern in patterns)
        {
            switch (pattern)
            {
                case "sequential characters":
                    feedback.Add($"Detected pattern: {pattern}.");
                    structuredFeedback.Add(new FeedbackItem(FeedbackCategory.SequentialChars, $"Detected pattern: {pattern}."));
                    break;
                case "repeated characters":
                    feedback.Add($"Detected pattern: {pattern}.");
                    structuredFeedback.Add(new FeedbackItem(FeedbackCategory.RepeatedChars, $"Detected pattern: {pattern}."));
                    break;
                case "keyboard walk":
                    feedback.Add($"Detected pattern: {pattern}.");
                    structuredFeedback.Add(new FeedbackItem(FeedbackCategory.KeyboardPattern, $"Detected pattern: {pattern}."));
                    break;
                case "common substitutions":
                    feedback.Add($"Detected pattern: {pattern}.");
                    structuredFeedback.Add(new FeedbackItem(FeedbackCategory.CommonSubstitution, $"Detected pattern: {pattern}."));
                    break;
                default:
                    feedback.Add($"Detected pattern: {pattern}.");
                    break;
            }
        }

        // Context-aware checking
        int contextPenalty = 0;
        if (context is not null)
        {
            contextPenalty = CheckContext(password, context, feedback, structuredFeedback);
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

        score = Math.Max(0, score - contextPenalty);

        string crackTime = EstimateCrackTime(effectiveEntropy);

        return new PasswordScore(score, Math.Round(entropy, 1), crackTime, feedback.ToArray())
        {
            StructuredFeedback = structuredFeedback
        };
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

    private static int CheckContext(string password, PasswordContext context, List<string> feedback, List<FeedbackItem> structuredFeedback)
    {
        var lower = password.ToLowerInvariant();
        int penalty = 0;

        if (!string.IsNullOrWhiteSpace(context.UserName) && context.UserName.Length >= 3
            && lower.Contains(context.UserName.ToLowerInvariant(), StringComparison.Ordinal))
        {
            feedback.Add("Password contains your username.");
            structuredFeedback.Add(new FeedbackItem(FeedbackCategory.ContextMatch, "Password contains your username."));
            penalty++;
        }

        if (!string.IsNullOrWhiteSpace(context.Email))
        {
            var emailLower = context.Email.ToLowerInvariant();
            var localPart = emailLower.Split('@')[0];

            if (localPart.Length >= 3 && lower.Contains(localPart, StringComparison.Ordinal))
            {
                feedback.Add("Password contains part of your email address.");
                structuredFeedback.Add(new FeedbackItem(FeedbackCategory.ContextMatch, "Password contains part of your email address."));
                penalty++;
            }
        }

        if (!string.IsNullOrWhiteSpace(context.FullName))
        {
            var nameParts = context.FullName.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in nameParts)
            {
                if (part.Length >= 3 && lower.Contains(part, StringComparison.Ordinal))
                {
                    feedback.Add($"Password contains your name (\"{part}\").");
                    structuredFeedback.Add(new FeedbackItem(FeedbackCategory.ContextMatch, $"Password contains your name (\"{part}\")."));
                    penalty++;
                    break;
                }
            }
        }

        if (context.DateOfBirth.HasValue)
        {
            var dob = context.DateOfBirth.Value;
            var dateFormats = new[]
            {
                dob.ToString("yyyyMMdd"),
                dob.ToString("MMddyyyy"),
                dob.ToString("ddMMyyyy"),
                dob.ToString("yyyy"),
                dob.ToString("MMdd"),
                dob.ToString("ddMM")
            };

            foreach (var format in dateFormats)
            {
                if (lower.Contains(format, StringComparison.Ordinal))
                {
                    feedback.Add("Password contains your date of birth.");
                    structuredFeedback.Add(new FeedbackItem(FeedbackCategory.ContextMatch, "Password contains your date of birth."));
                    penalty++;
                    break;
                }
            }
        }

        return penalty;
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
