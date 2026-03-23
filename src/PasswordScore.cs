namespace Philiprehberger.PasswordStrength;

/// <summary>
/// Represents the result of a password strength evaluation.
/// </summary>
/// <param name="Score">Strength score from 0 (very weak) to 4 (very strong).</param>
/// <param name="Entropy">Shannon entropy of the password in bits.</param>
/// <param name="CrackTime">Human-readable estimate of time to crack the password.</param>
/// <param name="Feedback">Array of improvement suggestions for the password.</param>
public record PasswordScore(int Score, double Entropy, string CrackTime, string[] Feedback);
