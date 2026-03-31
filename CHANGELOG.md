# Changelog

## 0.2.1 (2026-03-31)

- Standardize README to 3-badge format with emoji Support section
- Update CI actions to v5 for Node.js 24 compatibility

## 0.2.0 (2026-03-27)

- Add context-aware password checking (username, email, name exclusion)
- Add keyboard pattern detection for QWERTY layout sequences
- Add passphrase evaluation mode for word-based passwords
- Add structured feedback messages with categories and actionable suggestions

## 0.1.1 (2026-03-24)

- Expand README usage section with feature subsections

## 0.1.0 (2026-03-22)

- Initial release
- `Password.Evaluate` for comprehensive password strength scoring
- `Password.IsCommon` to check against top 1000 common passwords
- `Password.MeetsPolicy` for policy-based validation
- `PasswordScore` record with score, entropy, crack time, and feedback
- `EntropyCalculator` with Shannon entropy and pattern detection
- `PasswordPolicy` record for configurable validation rules
