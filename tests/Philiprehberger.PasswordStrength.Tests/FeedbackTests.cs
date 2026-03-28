using Xunit;
using Philiprehberger.PasswordStrength;

namespace Philiprehberger.PasswordStrength.Tests;

public class FeedbackTests
{
    [Fact]
    public void Evaluate_CommonPassword_HasCommonPasswordCategory()
    {
        var result = Password.Evaluate("password123");

        Assert.Contains(result.StructuredFeedback, f => f.Category == FeedbackCategory.CommonPassword);
        Assert.Contains(result.StructuredFeedback, f => f.Message.Contains("commonly used"));
    }

    [Fact]
    public void Evaluate_ShortPassword_HasTooShortCategory()
    {
        var result = Password.Evaluate("Ab1!");

        Assert.Contains(result.StructuredFeedback, f => f.Category == FeedbackCategory.TooShort);
    }

    [Fact]
    public void Evaluate_NoUppercase_HasLowDiversityCategory()
    {
        var result = Password.Evaluate("alllowercase1!");

        Assert.Contains(result.StructuredFeedback, f =>
            f.Category == FeedbackCategory.LowDiversity && f.Message.Contains("uppercase"));
    }

    [Fact]
    public void Evaluate_NoDigits_HasLowDiversityCategory()
    {
        var result = Password.Evaluate("NoDigitsHere!");

        Assert.Contains(result.StructuredFeedback, f =>
            f.Category == FeedbackCategory.LowDiversity && f.Message.Contains("numbers"));
    }

    [Fact]
    public void Evaluate_KeyboardPattern_HasKeyboardPatternCategory()
    {
        var result = Password.Evaluate("qwerty!A1xyz");

        Assert.Contains(result.StructuredFeedback, f => f.Category == FeedbackCategory.KeyboardPattern);
    }

    [Fact]
    public void Evaluate_EmptyPassword_HasTooShortCategory()
    {
        var result = Password.Evaluate("");

        Assert.Contains(result.StructuredFeedback, f => f.Category == FeedbackCategory.TooShort);
    }

    [Fact]
    public void FeedbackItem_HasCategoryAndMessage()
    {
        var item = new FeedbackItem(FeedbackCategory.CommonPassword, "Test message");

        Assert.Equal(FeedbackCategory.CommonPassword, item.Category);
        Assert.Equal("Test message", item.Message);
    }

    [Fact]
    public void Evaluate_StrongPassword_HasNoCommonPasswordFeedback()
    {
        var result = Password.Evaluate("Xk9!mP2vL#qR");

        Assert.DoesNotContain(result.StructuredFeedback, f => f.Category == FeedbackCategory.CommonPassword);
    }
}
