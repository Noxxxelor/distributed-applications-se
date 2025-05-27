using InternshipApp.Helpers;
using System.Collections.Generic;
using Common.Entities;

namespace InternshipApp.ViewModels
{
    public class ApplicationsListVM
    {
        public List<ApplicationVM> Applications { get; set; }
        public PaginatedList<Application> Pagination { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
    }
}
