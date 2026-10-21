using System.Windows;
using WpfApp2.Data;
using WpfApp2.Security;

namespace WpfApp2;

public partial class ChangePasswordWindow : Window
{
    private readonly int _userId;
    private readonly bool _requireChange;

    public ChangePasswordWindow(int userId, bool requireChange = false)
    {
        _userId = userId;
        _requireChange = requireChange;
        InitializeComponent();
        TextNotice.Text = requireChange
            ? "You must change the insecure default password to continue."
            : "Choose a new password of at least 8 characters.";
        if (requireChange) BtnCancel.Visibility = Visibility.Collapsed;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        string oldPassword = OldPassword.Password;
        string newPassword = NewPassword.Password;
        if (newPassword.Length < 8) { MessageBox.Show("New password must be at least 8 characters."); return; }
        if (newPassword != ConfirmPassword.Password) { MessageBox.Show("New passwords do not match."); return; }
        if (newPassword == oldPassword) { MessageBox.Show("Choose a different new password."); return; }
        try
        {
            using var db = new App_dbContext();
            var user = db.Users.Find(_userId);
            if (user is null || !PasswordHasher.Verify(oldPassword, user.Password, out _))
            {
                MessageBox.Show("Current password is incorrect.");
                return;
            }
            user.Password = PasswordHasher.Hash(newPassword);
            db.SaveChanges();
            MessageBox.Show("Password updated.");
            DialogResult = true;
        }
        catch (Exception ex) { MessageBox.Show($"Password change failed: {ex.Message}"); }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        if (!_requireChange) DialogResult = false;
    }
}
