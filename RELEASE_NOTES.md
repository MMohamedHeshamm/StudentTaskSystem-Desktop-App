# Improved Final 3 — change summary

Base: original `Final 3.rar` (`Final 2/WpfApp2/`)

- Fixed the known student-update and delete null issues.
- Refreshed assignment entities from a short-lived EF DbContext; separated status editing from the read-only grid.
- Added validation and confirmations.
- Implemented PBKDF2-SHA256 password hashing, legacy plaintext password upgrade on successful login, teacher setup and password-changing dialogs.
- Kept the original EF migration and `WPFProject1` schema.
- Updated WPF layouts and added startup instructions and verification checklist in README.

**Build verification is pending on Windows with Visual Studio and SQL Server.** No runnable EXE is included.
