using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using WpfApp2.Data;
using WpfApp2.Model;

namespace WpfApp2;

public partial class StudentAssignmentsWindow : Window
{
    private int? _selectedAssignmentId;
    private readonly int _studentId;
    private readonly bool _isStudentUser;
    private readonly int? _userId;

    public StudentAssignmentsWindow(int studentId, bool isStudentUser = false, int? userId = null)
    {
        _studentId = studentId;
        _isStudentUser = isStudentUser;
        _userId = userId;
        InitializeComponent();
        BtnAdd.Visibility = _isStudentUser ? Visibility.Collapsed : Visibility.Visible;
        BtnUpdate.Visibility = _isStudentUser ? Visibility.Collapsed : Visibility.Visible;
        BtnDelete.Visibility = _isStudentUser ? Visibility.Collapsed : Visibility.Visible;
        BtnChangePassword.Visibility = _isStudentUser && _userId.HasValue ? Visibility.Visible : Visibility.Collapsed;
        BtnLogOut.Visibility = _isStudentUser ? Visibility.Visible : Visibility.Collapsed;
        LoadStudentName();
        LoadAssignments();
    }

    private void LoadStudentName()
    {
        using var db = new App_dbContext();
        var student = db.Students.AsNoTracking().FirstOrDefault(s => s.StudentId == _studentId);
        TxtStudentName.Text = student is null
            ? "Student not found" : $"Assignments for: {student.StudentName} (ID: {student.StudentId})";
    }

    private void LoadAssignments()
    {
        _selectedAssignmentId = null;
        DgAssignments.ItemsSource = null;
        StatusPicker.SelectedIndex = -1;
        using var db = new App_dbContext();
        DgAssignments.ItemsSource = db.Assignments.AsNoTracking()
            .Where(a => a.StudentId == _studentId)
            .OrderBy(a => a.AssignmentId).ToList();
    }

    private void DgAssignments_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DgAssignments.SelectedItem is Assignment assignment)
        {
            _selectedAssignmentId = assignment.AssignmentId;
            StatusPicker.SelectedItem = StatusPicker.Items.OfType<ComboBoxItem>()
                .FirstOrDefault(item => string.Equals(item.Content?.ToString(), assignment.AssignmentStatus,
                    StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            _selectedAssignmentId = null;
            StatusPicker.SelectedIndex = -1;
        }
    }

    private void AddAssignment_Click(object sender, RoutedEventArgs e)
    {
        if (_isStudentUser) return;
        new AddEditAssignmentWindow(_studentId) { Owner = this }.ShowDialog();
        LoadAssignments();
    }

    private void UpdateAssignment_Click(object sender, RoutedEventArgs e)
    {
        if (_isStudentUser) return;
        if (_selectedAssignmentId is not int id) { MessageBox.Show("Select an assignment first."); return; }
        new AddEditAssignmentWindow(_studentId, id) { Owner = this }.ShowDialog();
        LoadAssignments();
    }

    private void DeleteAssignment_Click(object sender, RoutedEventArgs e)
    {
        if (_isStudentUser) return;
        if (_selectedAssignmentId is not int id) { MessageBox.Show("Select an assignment first."); return; }
        if (MessageBox.Show("Delete the selected assignment?", "Confirm deletion",
            MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
        try
        {
            using var db = new App_dbContext();
            var assignment = db.Assignments.FirstOrDefault(a => a.AssignmentId == id && a.StudentId == _studentId);
            if (assignment is null) { MessageBox.Show("Assignment no longer exists."); LoadAssignments(); return; }
            db.Assignments.Remove(assignment);
            db.SaveChanges();
            LoadAssignments();
        }
        catch (Exception ex) { MessageBox.Show($"Could not delete assignment: {ex.Message}"); }
    }

    private void UpdateStatus_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedAssignmentId is not int id) { MessageBox.Show("Select an assignment first."); return; }
        if (StatusPicker.SelectedItem is not ComboBoxItem option || option.Content is not string status)
        {
            MessageBox.Show("Choose a valid assignment status.");
            return;
        }
        try
        {
            using var db = new App_dbContext();
            // Never update an assignment belonging to another student.
            var assignment = db.Assignments.FirstOrDefault(a => a.AssignmentId == id && a.StudentId == _studentId);
            if (assignment is null) { MessageBox.Show("Assignment no longer exists."); LoadAssignments(); return; }
            assignment.AssignmentStatus = status;
            db.SaveChanges();
            LoadAssignments();
            MessageBox.Show("Assignment status saved.");
        }
        catch (Exception ex) { MessageBox.Show($"Could not update status: {ex.Message}"); }
    }

    private void ChangePassword_Click(object sender, RoutedEventArgs e)
    {
        if (!_isStudentUser || _userId is not int userId) return;
        new ChangePasswordWindow(userId) { Owner = this }.ShowDialog();
    }

    private void Log_Out_Click(object sender, RoutedEventArgs e)
    {
        if (!_isStudentUser) { Close(); return; }
        new AuthWindow().Show();
        Close();
    }
}
