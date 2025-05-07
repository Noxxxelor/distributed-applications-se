using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Common.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string University { get; set; } = null!;
        public string Degree { get; set; } = null!;
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}

