using Microsoft.EntityFrameworkCore;
using WpfApp2.Model;

namespace WpfApp2.Data;

public class App_dbContext : DbContext
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<User> Users => Set<User>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        // Supports different SQL Server installations without editing source code.
        string connection = Environment.GetEnvironmentVariable("STUDENT_TASK_DB_CONNECTION")
            ?? @"Server=localhost\SQLEXPRESS;Database=StudentAssignmentSystem-DesktopApp;Trusted_Connection=True;TrustServerCertificate=True;";
        optionsBuilder.UseSqlServer(connection);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>()
            .HasMany(s => s.Assignments)
            .WithOne(a => a.Student)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
        base.OnModelCreating(modelBuilder);
    }
}
