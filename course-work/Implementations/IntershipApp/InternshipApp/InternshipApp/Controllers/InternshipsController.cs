using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Common.Entities;
using Common.Models;     
using Common.Repositories; 
using InternshipApp.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using InternshipApp.Helpers;

namespace InternshipApp.Controllers
{
    [Authorize(Roles = "STUDENT")]
    public class InternshipsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InternshipsController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index(string searchTitle, string searchLocation, string sortOrder, int page = 1)
        {
            int pageSize = 5;

            var internshipsQuery = _context.Internships.Include(i => i.Company).AsQueryable();

            // Поиск
            if (!string.IsNullOrEmpty(searchTitle))
                internshipsQuery = internshipsQuery.Where(i => i.Title.Contains(searchTitle));
            if (!string.IsNullOrEmpty(searchLocation))
                internshipsQuery = internshipsQuery.Where(i => i.Location.Contains(searchLocation));

            // Сортировка
            ViewData["CurrentSort"] = sortOrder;
            ViewData["StartDateSortParm"] = string.IsNullOrEmpty(sortOrder) ? "start_desc" : "";
            ViewData["SalarySortParm"] = sortOrder == "Salary" ? "salary_desc" : "Salary";
            ViewData["TitleSortParm"] = sortOrder == "Title" ? "title_desc" : "Title";

            internshipsQuery = sortOrder switch
            {
                "start_desc" => internshipsQuery.OrderByDescending(i => i.StartDate),
                "Salary" => internshipsQuery.OrderBy(i => i.Salary),
                "salary_desc" => internshipsQuery.OrderByDescending(i => i.Salary),
                "Title" => internshipsQuery.OrderBy(i => i.Title),
                "title_desc" => internshipsQuery.OrderByDescending(i => i.Title),
                _ => internshipsQuery.OrderBy(i => i.StartDate)
            };

            var paginated = await PaginatedList<Internship>.CreateAsync(internshipsQuery, page, pageSize);

            var vmList = paginated.Select(i => new InternshipVM
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                StartDate = i.StartDate,
                EndDate = i.EndDate,
                Location = i.Location,
                Salary = i.Salary,
                CompanyId = i.CompanyId
            }).ToList();

            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == user.Email);

            var applicationsStatus = new Dictionary<int, ApplicationStatus>();
            if (student != null)
            {
                applicationsStatus = await _context.Applications
                    .Where(a => a.StudentId == student.Id)
                    .ToDictionaryAsync(a => a.InternshipId, a => a.Status);
            }

            var vm = new InternshipsIndexVM
            {
                Internships = vmList,
                ApplicationsStatus = applicationsStatus,
                Pagination = paginated,
                SearchTitle = searchTitle,
                SearchLocation = searchLocation
            };

            return View(vm);
        }


        public async Task<IActionResult> Details(int id)
        {
            var internship = await _context.Internships
                .Include(i => i.Company)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (internship == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == user.Email);

            ApplicationStatus? applicationStatus = null;
            if (student != null)
            {
                var application = await _context.Applications
                    .FirstOrDefaultAsync(a => a.InternshipId == id && a.StudentId == student.Id);
                if (application != null)
                    applicationStatus = application.Status;
            }

            var vm = new InternshipDetailsVM
            {
                Id = internship.Id,
                Title = internship.Title,
                Description = internship.Description,
                StartDate = internship.StartDate,
                EndDate = internship.EndDate,
                Location = internship.Location,
                Salary = internship.Salary,
                CompanyName = internship.Company?.Name,
                ApplicationStatus = applicationStatus
            };

            return View(vm);
        }

        
        public async Task<IActionResult> Apply(int id)
        {
            var internship = await _context.Internships.FindAsync(id);
            if (internship == null)
                return NotFound();

            var model = new ApplicationCreateVM
            {
                InternshipId = id
            };

            return View(model);
        }

        
        [HttpPost]
        public async Task<IActionResult> Apply(ApplicationCreateVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == user.Email);
            if (student == null)
                return Forbid();

            
            var existing = await _context.Applications.FirstOrDefaultAsync(a =>
                a.InternshipId == model.InternshipId && a.StudentId == student.Id);

            if (existing != null)
            {
                ModelState.AddModelError("", "Вы уже подавали заявку на эту стажировку.");
                return View(model);
            }

            var application = new Application
            {
                InternshipId = model.InternshipId,
                StudentId = student.Id,
                MotivationLetter = model.MotivationLetter,
                HasPortfolio = model.HasPortfolio,
                ApplicationDate = System.DateTime.UtcNow,
                Status = ApplicationStatus.Pending
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyApplications");
        }


        public async Task<IActionResult> MyApplications(string statusFilter, string sortOrder, int page = 1)
        {
            int pageSize = 5;

            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == user.Email);
            if (student == null) return Forbid();

            var appsQuery = _context.Applications
                .Include(a => a.Internship).ThenInclude(i => i.Company)
                .Where(a => a.StudentId == student.Id);

            // Фильтрация
            if (!string.IsNullOrEmpty(statusFilter) && Enum.TryParse<ApplicationStatus>(statusFilter, out var status))
            {
                appsQuery = appsQuery.Where(a => a.Status == status);
            }

            // Сортировка
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = sortOrder == "Title" ? "title_desc" : "Title";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";

            appsQuery = sortOrder switch
            {
                "Title" => appsQuery.OrderBy(a => a.Internship.Title),
                "title_desc" => appsQuery.OrderByDescending(a => a.Internship.Title),
                "Status" => appsQuery.OrderBy(a => a.Status),
                "status_desc" => appsQuery.OrderByDescending(a => a.Status),
                _ => appsQuery.OrderByDescending(a => a.ApplicationDate)
            };

            var paginated = await PaginatedList<Application>.CreateAsync(appsQuery, page, pageSize);

            var vm = new MyApplicationsVM
            {
                Applications = paginated.Select(a => new ApplicationVM
                {
                    Id = a.Id,
                    InternshipTitle = a.Internship?.Title,
                    Status = a.Status,
                    StudentName = student.FullName
                }).ToList(),
                Pagination = paginated,
                CurrentFilter = statusFilter
            };

            return View(vm);
        }

    }
}
