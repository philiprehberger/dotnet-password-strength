namespace Philiprehberger.PasswordStrength;

/// <summary>
/// Provides user-specific context for context-aware password evaluation.
/// Passwords containing any of these values (case-insensitive) will be penalized.
/// </summary>
public class PasswordContext
{
    /// <summary>
    /// Gets or sets the user's username.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the user's full name.
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Gets or sets the user's date of birth.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
}
