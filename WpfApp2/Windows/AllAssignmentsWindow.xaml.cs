using Microsoft.EntityFrameworkCore;
using System.Windows;
using WpfApp2.Data;

namespace WpfApp2;

public partial class AllAssignmentsWindow : Window
{
    public AllAssignmentsWindow()
    {
        InitializeComponent();
        LoadAll();
    }

    private void LoadAll()
    {
        using var db = new App_dbContext();
        AllAssignGrid.ItemsSource = db.Assignments.AsNoTracking()
            .Include(a => a.Student).OrderBy(a => a.AssignmentId).ToList();
    }
}
