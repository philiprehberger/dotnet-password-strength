using Xunit;
using Philiprehberger.PasswordStrength;

namespace Philiprehberger.PasswordStrength.Tests;

public class PassphraseEvaluatorTests
{
    [Fact]
    public void IsPassphrase_ThreeWords_ReturnsTrue()
    {
        Assert.True(PassphraseEvaluator.IsPassphrase("correct horse battery"));
    }

    [Fact]
    public void IsPassphrase_HyphenSeparated_ReturnsTrue()
    {
        Assert.True(PassphraseEvaluator.IsPassphrase("correct-horse-battery"));
    }

    [Fact]
    public void IsPassphrase_DotSeparated_ReturnsTrue()
    {
        Assert.True(PassphraseEvaluator.IsPassphrase("correct.horse.battery"));
    }

    [Fact]
    public void IsPassphrase_TwoWords_ReturnsFalse()
    {
        Assert.False(PassphraseEvaluator.IsPassphrase("correct horse"));
    }

    [Fact]
    public void IsPassphrase_SingleWord_ReturnsFalse()
    {
        Assert.False(PassphraseEvaluator.IsPassphrase("password"));
    }

    [Fact]
    public void Evaluate_FourUniqueWords_ScoresHighly()
    {
        var result = PassphraseEvaluator.Evaluate("correct horse battery staple");

        Assert.True(result.Score >= 3);
    }

    [Fact]
    public void Evaluate_FiveUniqueWords_ScoresMaximum()
    {
        var result = PassphraseEvaluator.Evaluate("correct horse battery staple dragon");

        Assert.Equal(4, result.Score);
    }

    [Fact]
    public void Evaluate_DuplicateWords_PenalizesScore()
    {
        var result = PassphraseEvaluator.Evaluate("horse horse horse horse");

        Assert.Contains(result.Feedback, f => f.Contains("repeating"));
    }

    [Fact]
    public void Evaluate_ThreeWords_SuggestsAddingFourth()
    {
        var result = PassphraseEvaluator.Evaluate("correct horse battery");

        Assert.Contains(result.Feedback, f => f.Contains("4th word"));
    }

    [Fact]
    public void Evaluate_PassphraseIntegration_DetectedByPasswordEvaluate()
    {
        var result = Password.Evaluate("correct horse battery staple");

        // Should be evaluated as a passphrase and score well
        Assert.True(result.Score >= 3);
    }
}
