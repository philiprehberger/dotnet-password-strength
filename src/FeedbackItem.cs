namespace Philiprehberger.PasswordStrength;

/// <summary>
/// A structured feedback message describing a specific weakness found in a password.
/// </summary>
/// <param name="Category">The category of the feedback.</param>
/// <param name="Message">A human-readable, actionable suggestion.</param>
public record FeedbackItem(FeedbackCategory Category, string Message);
