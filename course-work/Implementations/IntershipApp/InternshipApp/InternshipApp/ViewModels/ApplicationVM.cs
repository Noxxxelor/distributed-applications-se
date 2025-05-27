using System;
using Common.Entities;

namespace InternshipApp.ViewModels
{
    public class ApplicationVM
    {
        public int Id { get; set; }

        public string InternshipTitle { get; set; } 

        public string StudentName { get; set; } 

        public ApplicationStatus Status { get; set; }

        public DateTime StudentRegistrationDate { get; set; } 
    }
}
