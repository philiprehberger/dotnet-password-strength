using Xunit;
using Philiprehberger.PasswordStrength;

namespace Philiprehberger.PasswordStrength.Tests;

public class PasswordContextTests
{
    [Fact]
    public void Evaluate_WithUsername_PenalizesPasswordContainingUsername()
    {
        var context = new PasswordContext { UserName = "johndoe" };
        var result = Password.Evaluate("johndoe123!A", context);

        Assert.Contains(result.Feedback, f => f.Contains("username"));
        Assert.Contains(result.StructuredFeedback, f => f.Category == FeedbackCategory.ContextMatch);
    }

    [Fact]
    public void Evaluate_WithEmail_PenalizesPasswordContainingEmailLocalPart()
    {
        var context = new PasswordContext { Email = "alice@example.com" };
        var result = Password.Evaluate("alice2024!XY", context);

        Assert.Contains(result.Feedback, f => f.Contains("email"));
        Assert.Contains(result.StructuredFeedback, f => f.Category == FeedbackCategory.ContextMatch);
    }

    [Fact]
    public void Evaluate_WithFullName_PenalizesPasswordContainingNamePart()
    {
        var context = new PasswordContext { FullName = "John Smith" };
        var result = Password.Evaluate("smith2024!XY", context);

        Assert.Contains(result.Feedback, f => f.Contains("name"));
        Assert.Contains(result.StructuredFeedback, f => f.Category == FeedbackCategory.ContextMatch);
    }

    [Fact]
    public void Evaluate_WithDateOfBirth_PenalizesPasswordContainingBirthYear()
    {
        var context = new PasswordContext { DateOfBirth = new DateTime(1990, 5, 15) };
        var result = Password.Evaluate("pass1990!XYZ", context);

        Assert.Contains(result.Feedback, f => f.Contains("date of birth"));
        Assert.Contains(result.StructuredFeedback, f => f.Category == FeedbackCategory.ContextMatch);
    }

    [Fact]
    public void Evaluate_WithContext_NoMatchDoesNotPenalize()
    {
        var context = new PasswordContext { UserName = "johndoe", Email = "john@example.com" };
        var withContext = Password.Evaluate("Xk9!mP2vL#qR", context);
        var withoutContext = Password.Evaluate("Xk9!mP2vL#qR");

        Assert.Equal(withoutContext.Score, withContext.Score);
    }

    [Fact]
    public void Evaluate_WithContext_CaseInsensitiveMatch()
    {
        var context = new PasswordContext { UserName = "JohnDoe" };
        var result = Password.Evaluate("JOHNDOE123!A", context);

        Assert.Contains(result.Feedback, f => f.Contains("username"));
    }

    [Fact]
    public void Evaluate_WithContext_LowersScore()
    {
        var context = new PasswordContext { UserName = "admin" };
        var withContext = Password.Evaluate("Admin!2024Xyz", context);
        var withoutContext = Password.Evaluate("Admin!2024Xyz");

        Assert.True(withContext.Score < withoutContext.Score || withContext.Score == 0);
    }
}
