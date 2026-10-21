using Microsoft.EntityFrameworkCore;
using System.Windows;
using WpfApp2.Data;
using WpfApp2.Model;
using WpfApp2.Security;

namespace WpfApp2;

public partial class AuthWindow : Window
{
    public AuthWindow()
    {
        InitializeComponent();
        RefreshTeacherSetup();
    }

    private void RefreshTeacherSetup()
    {
        try
        {
            using var db = new App_dbContext();
            bool hasTeacher = db.Users.Any(u => u.Role == "Teacher");
            BtnCreateTeacher.Visibility = hasTeacher ? Visibility.Collapsed : Visibility.Visible;
            TxtSetupInfo.Text = hasTeacher
                ? "Students can register here. Teachers sign in with their existing account."
                : "First run: create a teacher account to manage students and assignments.";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Couldn't read the database. Check your SQL Server connection and run Update-Database.\n\n{ex.Message}",
                "Database connection error", MessageBoxButton.OK, MessageBoxImage.Error);
            BtnCreateTeacher.IsEnabled = false;
            BtnLogin.IsEnabled = false;
            BtnRegister.IsEnabled = false;
        }
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        string username = TxtUsername.Text.Trim();
        string password = textboxOfPassword.Password;
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Enter your username and password.");
            return;
        }
        try
        {
            using var db = new App_dbContext();
            var user = db.Users.FirstOrDefault(x => x.Username == username);
            if (user is null || !PasswordHasher.Verify(password, user.Password, out bool needsUpgrade))
            {
                MessageBox.Show("Incorrect username or password.");
                return;
            }

            if (needsUpgrade)
            {
                user.Password = PasswordHasher.Hash(password);
                db.SaveChanges();
            }

            if (user.Role == "Teacher")
            {
                // Final 3 originally installed a predictable teacher/123 account.
                if (password == "123")
                {
                    MessageBox.Show("For your security, change the original default password before continuing.");
                    var change = new ChangePasswordWindow(user.UserId, requireChange: true) { Owner = this };
                    if (change.ShowDialog() != true) return;
                }
                new MainWindow(user.UserId).Show();
            }
            else if (user.Role == "Student" && user.StudentId is int studentId)
            {
                if (!db.Students.Any(s => s.StudentId == studentId))
                {
                    MessageBox.Show("This account is not linked to a valid student. Contact your teacher.");
                    return;
                }
                new StudentAssignmentsWindow(studentId, isStudentUser: true, userId: user.UserId).Show();
            }
            else
            {
                MessageBox.Show("This account has an invalid role or a missing student link.");
                return;
            }
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Login failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RegisterStudent_Click(object sender, RoutedEventArgs e)
    {
        string username = TxtUsername.Text.Trim();
        string password = textboxOfPassword.Password;
        if (!ValidateNewCredentials(username, password)) return;
        try
        {
            using var db = new App_dbContext();
            if (db.Users.Any(x => x.Username == username))
            {
                MessageBox.Show("Username is already taken.");
                return;
            }
            // One SaveChanges: don't create an orphan student when account creation fails.
            var student = new Student { StudentName = username, StudentAge = 0 };
            db.Students.Add(student);
            db.Users.Add(new User
            {
                Username = username,
                Password = PasswordHasher.Hash(password),
                Role = "Student",
                Student = student
            });
            db.SaveChanges();
            MessageBox.Show("Student registered successfully. You can now log in.");
            textboxOfPassword.Clear();
        }
        catch (DbUpdateException)
        {
            MessageBox.Show("Registration failed. Check if the username already exists and try again.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Registration failed: {ex.Message}");
        }
    }

    private void CreateTeacher_Click(object sender, RoutedEventArgs e)
    {
        string username = TxtUsername.Text.Trim();
        string password = textboxOfPassword.Password;
        if (!ValidateNewCredentials(username, password)) return;
        try
        {
            using var db = new App_dbContext();
            if (db.Users.Any(u => u.Role == "Teacher"))
            {
                MessageBox.Show("A teacher account already exists.");
                RefreshTeacherSetup();
                return;
            }
            if (db.Users.Any(u => u.Username == username))
            {
                MessageBox.Show("Username is already taken.");
                return;
            }
            db.Users.Add(new User { Username = username, Password = PasswordHasher.Hash(password), Role = "Teacher" });
            db.SaveChanges();
            textboxOfPassword.Clear();
            RefreshTeacherSetup();
            MessageBox.Show("Teacher account created. Sign in to continue.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Couldn't create teacher: {ex.Message}");
        }
    }

    private static bool ValidateNewCredentials(string username, string password)
    {
        if (username.Length < 3 || username.Length > 50 || username.Any(char.IsWhiteSpace))
        {
            MessageBox.Show("Username must be 3–50 characters with no spaces.");
            return false;
        }
        if (password.Length < 8)
        {
            MessageBox.Show("Use a password with at least 8 characters.");
            return false;
        }
        return true;
    }
}
