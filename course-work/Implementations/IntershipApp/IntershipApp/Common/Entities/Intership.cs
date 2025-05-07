using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Common.Entities
{
    public class Internship
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}

