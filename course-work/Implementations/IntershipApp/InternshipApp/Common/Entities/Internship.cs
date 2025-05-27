using System;
using System.Collections.Generic;

namespace Common.Entities
{
    public class Internship
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Salary { get; set; }

        public string Location { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; }

        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}
