# Student Task Manager — Improved Final 3 (WPF)

A Windows desktop application built with **C# / .NET 10 / WPF**, **Entity Framework Core** and **SQL Server**. This is an improved version of the supplied `Final 3.rar` project; it keeps the original database schema and EF Core migration so that an existing `WPFProject1` database can be reused.

## Features

- Teacher sign-in, first-run teacher account setup, and student self-registration.
- Teacher dashboard: add, view, edit and delete students and their linked login accounts.
- Teacher assignment management: add, edit, delete and view assignments for each student.
- Student dashboard: see only their assignments and change their status (Pending, In Progress, Completed).
- List all assignments with student names.
- Password changes for both teachers and students.
- Improved field validation, delete confirmations, and refreshed EF Core data without stale entity tracking.
- Passwords are stored with PBKDF2-SHA256 and a randomly generated salt. **Legacy plaintext passwords from Final 3 are upgraded when their users log in successfully.**

## Requirements

- Windows 10/11
- Visual Studio 2026 (or a Visual Studio edition that supports **.NET 10 WPF**) with the **.NET desktop development** workload and .NET 10 SDK
- SQL Server (Express/Developer/other local instance) and optional SSMS
- Internet during the first NuGet restore (unless packages are already cached)

## Quick start

1. Open `WpfApp2.slnx` in Visual Studio.
2. Set `StudentTaskSystem` as the startup project.
3. Check your SQL Server instance. The default is `localhost\SQLEXPRESS`, and database name is `WPFProject1`.
4. If your instance differs, set the environment variable **`STUDENT_TASK_DB_CONNECTION`** before starting Visual Studio. For example, in PowerShell:

   ```powershell
   $env:STUDENT_TASK_DB_CONNECTION = 'Server=.;Database=WPFProject1;Trusted_Connection=True;TrustServerCertificate=True;'
   ```

   An environment variable set only in a PowerShell session is inherited by Visual Studio **only when you start Visual Studio from that same session**. Alternatively set it in Windows *Environment Variables* and restart Visual Studio, or edit `Data/App_dbContext.cs` for your local development machine.

5. From **Tools → NuGet Package Manager → Package Manager Console**, select `StudentTaskSystem` as the default project, then run:

   ```powershell
   Update-Database
   ```

   The existing migration is included. If you already have the original `WPFProject1` database with its `__EFMigrationsHistory` table, **back up the database first** and do not delete it: this version is designed to use the same tables.

6. Build the solution and press **F5**.
7. If no teacher exists, use **Create Teacher** on the sign-in screen (a password of at least 8 characters is required). If you already have an account, log in normally.

> **Existing Final 3 users:** Student and teacher passwords from the old database still work. On successful login, the old plaintext password will be replaced by a salted PBKDF2 hash. The old insecure `teacher / 123` account, if it exists, is required to change its password upon login. **New installations no longer automatically create that default account.**

## Notes on the database

- EF Core migration `20251127112109_first` is deliberately unchanged: changes to password handling fit in the existing `Users.Password` string column.
- Student registration starts with age `0` (unspecified), just as in the original app; the teacher must enter an age from 1 to 120 when editing it.
- Removing a student also removes their linked user account and assignments. Confirm before deletion.
- Current app is a **desktop learning/demo project**, not a hardened multi-user production system. The SQL Server connection runs with Windows authentication and the current client's DB permissions; authorization is UI-level rather than enforced by a server API. Use a proper backend service, restricted DB rights, unique username constraint, and additional security controls before any real deployment.
- If `Update-Database` fails, check SQL Server service, instance name, Windows authentication access, and NuGet restoration. Do not disable NuGet security auditing as a permanent fix.

## Changes from Final 3

- Fixed null handling when removing students without linked login accounts.
- Replaced unsafe `First()` lookups with existence checks.
- Added required-field, age and credential validation to avoid silently setting an invalid age to `0`.
- Removed long-lived DbContext objects from WPF windows, reducing EF entity tracking conflicts and stale displays.
- Added confirmation before student/assignment deletion.
- Separated status selection from read-only assignment DataGrid, fixing update/refresh inconsistencies.
- Stopped showing saved passwords in the student editing form.
- Added hashing, initial teacher setup, legacy-password upgrade and change-password flows.
- Added clearer WPF layouts and user-friendly database error messages.

## Verification status

The XAML files were parsed successfully and UI event-handler names were checked against code. **Compilation, WPF runtime behavior, SQL Server access and database migration were not tested in this environment**, which does not provide a Windows/.NET 10 desktop runtime. Please run `Build → Rebuild Solution` and test the checklist below on Windows before publishing this repository as a completed project.

### Windows smoke-test checklist

- [ ] Run `Update-Database` with a fresh local SQL Server database.
- [ ] Create a teacher on a new database; sign in; change password.
- [ ] Register a student; sign in; change password.
- [ ] Teacher: add, edit and delete students, including a student with assignments.
- [ ] Teacher: add, edit and delete assignments.
- [ ] Student: update assignment status; close/reopen window; confirm new status persists.
- [ ] Reject bad age input, empty names, duplicate usernames and short passwords.
- [ ] If using an original Final 3 database, verify legacy login and automatic password upgrade.
- [ ] Verify teacher account originally using `123` is required to change its password.

## Repository suggestion

Use `StudentTaskManager-WPF` as the GitHub repository name. The project keeps the original `WpfApp2` C# namespace to preserve compatibility with existing WPF XAML and EF migrations.
