using Common.Entities;
using Common.Models;
using InternshipApp.Helpers;

namespace InternshipApp.ViewModels
{
    public class AdminDashboardVM
    {
        public PaginatedList<ApplicationUser> Users { get; set; }
        public PaginatedList<Student> Students { get; set; }
        public PaginatedList<Company> Companies { get; set; }

        public string UserSortOrder { get; set; }
        public string UserSearchString { get; set; }
        public string StudentSortOrder { get; set; }
        public string StudentSearchString { get; set; }
        public string CompanySortOrder { get; set; }
        public string CompanySearchString { get; set; }
    }
}
