using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Common.Entities;
using InternshipApp.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using Common.Repositories;
using InternshipApp.Helpers;


namespace InternshipApp.Controllers
{
    [Authorize(Roles = "COMPANY")]
    public class CompanyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompanyController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        public async Task<IActionResult> MyInternships(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            var user = await _userManager.GetUserAsync(User);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.ContactEmail == user.Email);
            if (company == null)
                return Forbid();

            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["StartDateSortParm"] = sortOrder == "StartDate" ? "startDate_desc" : "StartDate";

            if (searchString != null)
                pageNumber = 1;
            else
                searchString = currentFilter;

            ViewData["CurrentFilter"] = searchString;

            var internships = _context.Internships
                .Where(i => i.CompanyId == company.Id);

            if (!string.IsNullOrEmpty(searchString))
            {
                internships = internships.Where(i =>
                    i.Title.Contains(searchString) ||
                    i.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    internships = internships.OrderByDescending(i => i.Title);
                    break;
                case "StartDate":
                    internships = internships.OrderBy(i => i.StartDate);
                    break;
                case "startDate_desc":
                    internships = internships.OrderByDescending(i => i.StartDate);
                    break;
                default:
                    internships = internships.OrderBy(i => i.Title);
                    break;
            }

            int pageSize = 5;

            return View(await PaginatedList<Internship>.CreateAsync(internships.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public IActionResult CreateInternship()
        {
            return View(new InternshipVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInternship(InternshipVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.ContactEmail == user.Email);
            if (company == null)
                return Forbid();

            model.CompanyId = company.Id;

            if (!ModelState.IsValid)
                return View(model);

            var internship = new Internship
            {
                Title = model.Title,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Location = model.Location,
                Salary = model.Salary,
                CompanyId = model.CompanyId
            };

            _context.Internships.Add(internship);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyInternships));
        }

        public async Task<IActionResult> Applications(int internshipId)
        {
            var user = await _userManager.GetUserAsync(User);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.ContactEmail == user.Email);
            if (company == null)
                return Forbid();

            var internship = await _context.Internships
                .Include(i => i.Applications)
                .ThenInclude(a => a.Student)
                .FirstOrDefaultAsync(i => i.Id == internshipId && i.CompanyId == company.Id);

            if (internship == null)
                return NotFound();

            var applicationVMs = internship.Applications.Select(a => new ApplicationVM
            {
                Id = a.Id,
                StudentName = a.Student.FullName,
                InternshipTitle = internship.Title,
                Status = a.Status
            }).ToList();

            return View(applicationVMs);
        }

        public async Task<IActionResult> ApplicationDetails(int id)
        {
            var application = await _context.Applications
                .Include(a => a.Student)
                .Include(a => a.Internship)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.ContactEmail == user.Email);

            if (company == null || application.Internship.CompanyId != company.Id)
                return Forbid();

            return View(application);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int applicationId, ApplicationStatus status)
        {
            var application = await _context.Applications
                .Include(a => a.Internship)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.ContactEmail == user.Email);

            if (company == null || application.Internship.CompanyId != company.Id)
                return Forbid();

            application.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction("Applications", new { internshipId = application.InternshipId });
        }
    }
}
