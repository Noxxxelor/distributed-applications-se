using System.Collections.Generic;
using Common.Entities;
using InternshipApp.Helpers;

namespace InternshipApp.ViewModels
{
    public class MyApplicationsVM
    {
        public List<ApplicationVM> Applications { get; set; }
        public PaginatedList<Application> Pagination { get; set; }
        public string CurrentFilter { get; set; }
    }
}
