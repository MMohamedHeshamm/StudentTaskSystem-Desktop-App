using System.Collections.Generic;

namespace WpfApp2.Model
{
    public class Student
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = "";
        public int StudentAge { get; set; }


        public User? User { get; set; } // nav prop of user 1 : 1

        public List<Assignment> Assignments { get; set; } = new(); // nav prop of assignment
    }
}
