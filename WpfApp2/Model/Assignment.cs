using WpfApp2.Model;

namespace WpfApp2.Model
{
    public class Assignment
    {
        public int AssignmentId { get; set; }
        public string AssignmentTitle { get; set; } = "";
        public string AssignmentDesc { get; set; } = "";
        public string AssignmentStatus { get; set; } = "";

        public int StudentId { get; set; } // FK
        public Student? Student { get; set; } // NAV prop


    }
}
