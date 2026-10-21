using System.Windows;
using WpfApp2.Data;
using WpfApp2.Model;

namespace WpfApp2;

public partial class AddEditAssignmentWindow : Window
{
    private readonly int _studentId;
    private readonly int? _assignmentId;

    public AddEditAssignmentWindow(int studentId, int? assignmentId = null)
    {
        _studentId = studentId;
        _assignmentId = assignmentId;
        InitializeComponent();
        Title = assignmentId.HasValue ? "Edit Assignment" : "Add Assignment";
        BtnSave.Content = assignmentId.HasValue ? "Save Changes" : "Add Assignment";
        if (_assignmentId.HasValue) LoadAssignment();
    }

    private void LoadAssignment()
    {
        using var db = new App_dbContext();
        var assignment = db.Assignments.FirstOrDefault(a => a.AssignmentId == _assignmentId && a.StudentId == _studentId);
        if (assignment is null) { MessageBox.Show("Assignment not found."); Loaded += (_, _) => Close(); return; }
        TxtTitle.Text = assignment.AssignmentTitle;
        TxtDesc.Text = assignment.AssignmentDesc;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        string title = TxtTitle.Text.Trim();
        string description = TxtDesc.Text.Trim();
        if (title.Length is < 1 or > 150)
        {
            MessageBox.Show("Assignment title must be 1–150 characters.");
            return;
        }
        if (description.Length > 2000)
        {
            MessageBox.Show("Description must be at most 2,000 characters.");
            return;
        }
        try
        {
            using var db = new App_dbContext();
            if (_assignmentId is int id)
            {
                var assignment = db.Assignments.FirstOrDefault(a => a.AssignmentId == id && a.StudentId == _studentId);
                if (assignment is null) { MessageBox.Show("Assignment not found."); return; }
                assignment.AssignmentTitle = title;
                assignment.AssignmentDesc = description;
            }
            else
            {
                if (!db.Students.Any(s => s.StudentId == _studentId))
                { MessageBox.Show("Student not found."); return; }
                db.Assignments.Add(new Assignment
                {
                    AssignmentTitle = title, AssignmentDesc = description,
                    AssignmentStatus = "Pending", StudentId = _studentId
                });
            }
            db.SaveChanges();
            DialogResult = true;
        }
        catch (Exception ex) { MessageBox.Show($"Could not save assignment: {ex.Message}"); }
    }
}
