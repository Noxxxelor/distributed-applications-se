using System;
using System.Collections.Generic;

namespace Common.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string ContactEmail { get; set; }
        public string HeadquartersAddress { get; set; }
        public string CompanyPhone { get; set; }
        public DateTime EstablishedDate { get; set; }
        public bool IsVerified { get; set; }

        public ICollection<Internship> Internships { get; set; } = new List<Internship>();
    }
}



