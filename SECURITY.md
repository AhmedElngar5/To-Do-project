# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |

## Reporting a Vulnerability

If you discover a security vulnerability within Ahmed OS, please do not open a public GitHub issue.
Instead, report security concerns directly to Ahmed Hany Kamal El Nagar via GitHub or email.

### Security Best Practices Implemented
- **ASP.NET Core Identity** with PBKDF2 hashed passwords and strict lockout policies.
- **Data Isolation**: All queries enforce user-scoped filtering (`UserId == currentUserId`).
- **Global Query Filters**: Soft-delete pattern implemented for data integrity.
- **CSRF Protection**: Antiforgery tokens validated on state-modifying requests.
- **Configurable Secrets**: Environment variable support to prevent credential leakage.
- **Minimal Surface**: HTTPS redirection and strict security headers enabled for production.
