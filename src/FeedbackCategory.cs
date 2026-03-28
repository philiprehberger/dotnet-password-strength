namespace Philiprehberger.PasswordStrength;

/// <summary>
/// Categories for structured password feedback messages.
/// </summary>
public enum FeedbackCategory
{
    /// <summary>The password matches a commonly used password.</summary>
    CommonPassword,

    /// <summary>The password contains a keyboard pattern such as "qwerty" or "asdfgh".</summary>
    KeyboardPattern,

    /// <summary>The password is too short.</summary>
    TooShort,

    /// <summary>The password lacks character diversity (missing uppercase, digits, or symbols).</summary>
    LowDiversity,

    /// <summary>The password contains user context information (username, email, name).</summary>
    ContextMatch,

    /// <summary>The password contains sequential characters.</summary>
    SequentialChars,

    /// <summary>The password contains repeated characters.</summary>
    RepeatedChars,

    /// <summary>The password uses common character substitutions.</summary>
    CommonSubstitution,

    /// <summary>The password could benefit from being longer.</summary>
    CouldBeLonger,

    /// <summary>The password is a passphrase with too few words.</summary>
    FewWords
}
