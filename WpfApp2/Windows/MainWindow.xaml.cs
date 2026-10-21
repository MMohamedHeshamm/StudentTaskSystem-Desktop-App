using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp2.Data;
using WpfApp2.Model;
using WpfApp2.Security;

namespace WpfApp2;

public partial class MainWindow : Window
{
    private int? _selectedStudentId;
    private readonly int _teacherUserId;

    public MainWindow(int teacherUserId)
    {
        _teacherUserId = teacherUserId;
        InitializeComponent();
        LoadData();
    }

    private void LoadData()
    {
        using var db = new App_dbContext();
        DgStudent.ItemsSource = db.Students.AsNoTracking().Include(s => s.User)
            .OrderBy(s => s.StudentId).ToList();
    }

    private void ClearInputs()
    {
        _selectedStudentId = null;
        DgStudent.SelectedItem = null;
        TextBoxOfName.Clear();
        TextBoxOfAge.Clear();
        TextBoxUser.Clear();
        PasswordBoxPass.Clear();
    }

    private static bool TryValidateStudent(string name, string ageText, string username,
        out int age)
    {
        age = 0;
        if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
        {
            MessageBox.Show("Enter a student name (up to 100 characters).");
            return false;
        }
        if (!int.TryParse(ageText, out age) || age < 1 || age > 120)
        {
            MessageBox.Show("Age must be a whole number from 1 to 120.");
            return false;
        }
        if (username.Length < 3 || username.Length > 50 || username.Any(char.IsWhiteSpace))
        {
            MessageBox.Show("Username must be 3–50 characters without spaces.");
            return false;
        }
        return true;
    }

    private void Add_Button_Click(object sender, RoutedEventArgs e)
    {
        string name = TextBoxOfName.Text.Trim();
        string username = TextBoxUser.Text.Trim();
        string password = PasswordBoxPass.Password;
        if (!TryValidateStudent(name, TextBoxOfAge.Text, username, out int age)) return;
        if (password.Length < 8)
        {
            MessageBox.Show("Password must contain at least 8 characters.");
            return;
        }
        try
        {
            using var db = new App_dbContext();
            if (db.Users.Any(u => u.Username == username))
            {
                MessageBox.Show("Username already exists.");
                return;
            }
            var student = new Student { StudentName = name, StudentAge = age };
            db.Students.Add(student);
            db.Users.Add(new User
            {
                Username = username, Password = PasswordHasher.Hash(password),
                Role = "Student", Student = student
            });
            db.SaveChanges();
            LoadData();
            ClearInputs();
        }
        catch (Exception ex) { MessageBox.Show($"Could not add student: {ex.Message}"); }
    }

    private void DgStudent_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DgStudent.SelectedItem is not Student student)
        {
            _selectedStudentId = null;
            return;
        }
        _selectedStudentId = student.StudentId;
        TextBoxOfName.Text = student.StudentName;
        TextBoxOfAge.Text = student.StudentAge == 0 ? "" : student.StudentAge.ToString();
        TextBoxUser.Text = student.User?.Username ?? "";
        PasswordBoxPass.Clear(); // Never disclose a saved password or password hash.
    }

    private void Update_Button_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedStudentId is not int studentId)
        {
            MessageBox.Show("Select a student first.");
            return;
        }
        string name = TextBoxOfName.Text.Trim();
        string username = TextBoxUser.Text.Trim();
        string password = PasswordBoxPass.Password;
        if (!TryValidateStudent(name, TextBoxOfAge.Text, username, out int age)) return;
        if (password.Length is > 0 and < 8)
        {
            MessageBox.Show("A new password must be at least 8 characters, or leave it blank to keep the existing one.");
            return;
        }
        try
        {
            using var db = new App_dbContext();
            var student = db.Students.Include(s => s.User).FirstOrDefault(s => s.StudentId == studentId);
            if (student is null) { MessageBox.Show("Student not found."); LoadData(); ClearInputs(); return; }
            var user = student.User;
            if (user is null)
            {
                MessageBox.Show("This student has no linked login account; edit their account in the database first.");
                return;
            }
            if (db.Users.Any(u => u.Username == username && u.UserId != user.UserId))
            {
                MessageBox.Show("Username already exists.");
                return;
            }
            student.StudentName = name;
            student.StudentAge = age;
            user.Username = username;
            if (!string.IsNullOrEmpty(password)) user.Password = PasswordHasher.Hash(password);
            db.SaveChanges();
            LoadData(); ClearInputs();
            MessageBox.Show("Student updated successfully.");
        }
        catch (Exception ex) { MessageBox.Show($"Could not update student: {ex.Message}"); }
    }

    private void Delete_Button_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedStudentId is not int studentId)
        {
            MessageBox.Show("Select a student first.");
            return;
        }
        if (MessageBox.Show("Delete this student and all their assignments? This cannot be undone.",
            "Confirm deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
        try
        {
            using var db = new App_dbContext();
            var student = db.Students.Include(s => s.User).FirstOrDefault(s => s.StudentId == studentId);
            if (student is null) { LoadData(); ClearInputs(); return; }
            if (student.User is not null) db.Users.Remove(student.User);
            db.Students.Remove(student); // Assignments are deleted via database cascade FK.
            db.SaveChanges();
            LoadData(); ClearInputs();
        }
        catch (Exception ex) { MessageBox.Show($"Could not delete student: {ex.Message}"); }
    }

    private void ShowAssignments_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedStudentId is not int studentId)
        {
            MessageBox.Show("Select a student first.");
            return;
        }
        new StudentAssignmentsWindow(studentId, isStudentUser: false) { Owner = this }.ShowDialog();
        LoadData(); ClearInputs();
    }

    private void ShowAllAssignments_Click(object sender, RoutedEventArgs e) =>
        new AllAssignmentsWindow { Owner = this }.ShowDialog();

    private void ChangePassword_Click(object sender, RoutedEventArgs e) =>
        new ChangePasswordWindow(_teacherUserId) { Owner = this }.ShowDialog();

    private void Clear_Button_Click(object sender, RoutedEventArgs e) => ClearInputs();

    private void LogOut_Click(object sender, RoutedEventArgs e)
    {
        new AuthWindow().Show();
        Close();
    }
}
