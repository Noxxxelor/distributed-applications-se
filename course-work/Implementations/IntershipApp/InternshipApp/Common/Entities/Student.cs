using System;
using System.Collections.Generic;

namespace Common.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string University { get; set; }
        public string Degree { get; set; }
        public DateTime RegistrationDate { get; set; } 

        public bool IsActive { get; set; } = true; 

        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}
