using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParonApp
{
    internal class Student
    {
        public Student(int studentID, string firstName, string lastName, bool isPresent)
        {
            StudentID = studentID;
            FirstName = firstName;
            LastName = lastName;
            IsPresent = isPresent;
        }
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsPresent { get; set; }
    }
}
