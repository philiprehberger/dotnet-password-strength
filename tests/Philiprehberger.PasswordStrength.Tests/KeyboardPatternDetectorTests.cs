using Xunit;
using Philiprehberger.PasswordStrength;

namespace Philiprehberger.PasswordStrength.Tests;

public class KeyboardPatternDetectorTests
{
    [Fact]
    public void Detect_QwertyPattern_ReturnsMatch()
    {
        var patterns = KeyboardPatternDetector.Detect("qwerty123");

        Assert.NotEmpty(patterns);
        Assert.Contains(patterns, p => p.Contains("qwerty"));
    }

    [Fact]
    public void Detect_AsdfghPattern_ReturnsMatch()
    {
        var patterns = KeyboardPatternDetector.Detect("asdfgh!");

        Assert.NotEmpty(patterns);
        Assert.Contains(patterns, p => p.Contains("asdfgh"));
    }

    [Fact]
    public void Detect_ZxcvbnPattern_ReturnsMatch()
    {
        var patterns = KeyboardPatternDetector.Detect("zxcvbn99");

        Assert.NotEmpty(patterns);
        Assert.Contains(patterns, p => p.Contains("zxcvbn"));
    }

    [Fact]
    public void Detect_QazwsxPattern_ReturnsMatch()
    {
        var patterns = KeyboardPatternDetector.Detect("qazwsx!!");

        Assert.NotEmpty(patterns);
        Assert.Contains(patterns, p => p.Contains("qazwsx"));
    }

    [Fact]
    public void Detect_NumericSequence_ReturnsMatch()
    {
        var patterns = KeyboardPatternDetector.Detect("pass12345word");

        Assert.NotEmpty(patterns);
        Assert.Contains(patterns, p => p.Contains("12345"));
    }

    [Fact]
    public void Detect_NoPattern_ReturnsEmpty()
    {
        var patterns = KeyboardPatternDetector.Detect("Xk9!mP2v");

        Assert.Empty(patterns);
    }

    [Fact]
    public void Detect_ShortInput_ReturnsEmpty()
    {
        var patterns = KeyboardPatternDetector.Detect("abc");

        Assert.Empty(patterns);
    }

    [Fact]
    public void Detect_CaseInsensitive_MatchesUppercase()
    {
        var patterns = KeyboardPatternDetector.Detect("QWERTY123");

        Assert.NotEmpty(patterns);
    }
}
