using System.Collections.Generic;
using Common.Entities;
using InternshipApp.Helpers;

namespace InternshipApp.ViewModels
{
    public class InternshipsIndexVM
    {
        public List<InternshipVM> Internships { get; set; }
        public Dictionary<int, ApplicationStatus> ApplicationsStatus { get; set; }

        public string SearchTitle { get; set; }
        public string SearchLocation { get; set; }
        public PaginatedList<Internship> Pagination { get; set; }
    }
}
