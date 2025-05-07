using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Industry { get; set; } = null!;
        public string ContactEmail { get; set; } = null!;
        public ICollection<Internship> Internships { get; set; } = new List<Internship>();
    }
}

