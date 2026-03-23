namespace Philiprehberger.PasswordStrength;

/// <summary>
/// Defines a password policy with minimum requirements for length, strength score, and common password rejection.
/// </summary>
/// <param name="MinLength">The minimum required password length. Default is 8.</param>
/// <param name="MinScore">The minimum required strength score (0-4). Default is 2.</param>
/// <param name="RejectCommon">Whether to reject passwords found in the common passwords list. Default is <c>true</c>.</param>
public record PasswordPolicy(int MinLength = 8, int MinScore = 2, bool RejectCommon = true);
