using Common.Entities;

namespace InternshipApp.ViewModels
{
    public class InternshipDetailsVM
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Salary { get; set; }

        public string Location { get; set; }

        public string CompanyName { get; set; }

        public ApplicationStatus? ApplicationStatus { get; set; }
    }
}
