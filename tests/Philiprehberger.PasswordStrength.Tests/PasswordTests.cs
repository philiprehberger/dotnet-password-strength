using Xunit;
using Philiprehberger.PasswordStrength;

namespace Philiprehberger.PasswordStrength.Tests;

public class PasswordTests
{
    [Fact]
    public void Evaluate_EmptyPassword_ReturnsZeroScore()
    {
        var result = Password.Evaluate("");

        Assert.Equal(0, result.Score);
        Assert.Equal(0.0, result.Entropy);
        Assert.Equal("instant", result.CrackTime);
    }

    [Fact]
    public void Evaluate_CommonPassword_ReturnsLowScore()
    {
        var result = Password.Evaluate("password");

        Assert.True(result.Score <= 1);
    }

    [Fact]
    public void Evaluate_StrongPassword_ReturnsHighScore()
    {
        var result = Password.Evaluate("Xk9!mP2vL#qR");

        Assert.True(result.Score >= 3);
    }

    [Fact]
    public void IsCommon_KnownPassword_ReturnsTrue()
    {
        Assert.True(Password.IsCommon("password123"));
    }

    [Fact]
    public void IsCommon_UniquePassword_ReturnsFalse()
    {
        Assert.False(Password.IsCommon("Xk9!mP2vL#qR"));
    }

    [Fact]
    public void MeetsPolicy_StrongPassword_ReturnsTrue()
    {
        var policy = new PasswordPolicy(MinLength: 8, MinScore: 2, RejectCommon: true);

        Assert.True(Password.MeetsPolicy("Xk9!mP2vL#qR", policy));
    }

    [Fact]
    public void MeetsPolicy_ShortPassword_ReturnsFalse()
    {
        var policy = new PasswordPolicy(MinLength: 12, MinScore: 2, RejectCommon: true);

        Assert.False(Password.MeetsPolicy("short", policy));
    }

    [Fact]
    public void Evaluate_ReturnsStructuredFeedback()
    {
        var result = Password.Evaluate("abc");

        Assert.NotNull(result.StructuredFeedback);
        Assert.NotEmpty(result.StructuredFeedback);
    }

    [Fact]
    public void Evaluate_NullPassword_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Password.Evaluate(null!));
    }
}
