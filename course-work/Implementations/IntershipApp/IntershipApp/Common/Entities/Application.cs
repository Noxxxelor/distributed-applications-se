using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    public class Application
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; } 

        public int InternshipId { get; set; }
        public Internship Internship { get; set; } 

        public DateTime ApplicationDate { get; set; } 
        public ApplicationStatus Status { get; set; } 
    }

    public enum ApplicationStatus
    {
        Pending,    
        Accepted,   
        Rejected    
    }
}

