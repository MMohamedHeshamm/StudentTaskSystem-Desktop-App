namespace WpfApp2.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";

        public string Role { get; set; } = "";

    
        public int? StudentId { get; set; } // FK
        public Student? Student { get; set; }  // NAV prop
    }
}
