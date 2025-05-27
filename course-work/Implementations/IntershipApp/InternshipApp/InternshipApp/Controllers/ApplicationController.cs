using Common.Entities;
using Common.Models;
using Common.Repositories;
using InternshipApp.Helpers;
using InternshipApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InternshipApp.Controllers
{
    [Authorize(Roles = "STUDENT,COMPANY")]
    public class ApplicationController : Controller
    {
        private readonly BaseRepository<Application> _applicationRepo;
        private readonly BaseRepository<Student> _studentRepo;
        private readonly BaseRepository<Internship> _internshipRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _applicationRepo = new BaseRepository<Application>(context);
            _studentRepo = new BaseRepository<Student>(context);
            _internshipRepo = new BaseRepository<Internship>(context);
            _userManager = userManager;
        }

        // Список заявок студента с поиском, сортировкой и пагинацией
        public async Task<IActionResult> Index(string searchTitle, string sortOrder, int pageNumber = 1)
        {
            int pageSize = 5;

            var user = await _userManager.GetUserAsync(User);
            var student = await _studentRepo.GetAllQueryable().FirstOrDefaultAsync(s => s.Email == user.Email);
            if (student == null)
                return Forbid();

            var query = _applicationRepo.GetAllQueryable()
                .Include(a => a.Internship)
                .Where(a => a.StudentId == student.Id);

            // Поиск
            if (!string.IsNullOrWhiteSpace(searchTitle))
            {
                query = query.Where(a => a.Internship.Title.Contains(searchTitle));
            }

            // Сортировка
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = sortOrder == "Title" ? "title_desc" : "Title";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            query = sortOrder switch
            {
                "Title" => query.OrderBy(a => a.Internship.Title),
                "title_desc" => query.OrderByDescending(a => a.Internship.Title),
                "Status" => query.OrderBy(a => a.Status),
                "status_desc" => query.OrderByDescending(a => a.Status),
                "Date" => query.OrderBy(a => a.ApplicationDate),
                "date_desc" => query.OrderByDescending(a => a.ApplicationDate),
                _ => query.OrderByDescending(a => a.ApplicationDate)
            };

            var pagedList = await PaginatedList<Application>.CreateAsync(query, pageNumber, pageSize);

            var vmList = pagedList.Select(a => new ApplicationVM
            {
                Id = a.Id,
                InternshipTitle = a.Internship?.Title,
                Status = a.Status,
                StudentName = student.FullName
            }).ToList();

            var vm = new ApplicationsListVM
            {
                Applications = vmList,
                Pagination = pagedList,
                CurrentFilter = searchTitle,
                CurrentSort = sortOrder
            };

            return View(vm);
        }

        // GET: Заявка на стажировку
        [HttpGet]
        [Authorize(Roles = "STUDENT")]
        public IActionResult Apply(int internshipId)
        {
            var internship = _internshipRepo.GetById(internshipId);
            if (internship == null)
                return NotFound();

            var vm = new ApplicationCreateVM
            {
                InternshipId = internshipId
            };

            return View(vm);
        }

        // POST: Заявка на стажировку
        [HttpPost]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> Apply(ApplicationCreateVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _userManager.GetUserAsync(User);
            var student = await _studentRepo.GetAllQueryable().FirstOrDefaultAsync(s => s.Email == user.Email);
            if (student == null)
                return Forbid();

            var exists = await _applicationRepo.GetAllQueryable()
                .AnyAsync(a => a.InternshipId == vm.InternshipId && a.StudentId == student.Id);

            if (exists)
            {
                ModelState.AddModelError("", "Вы уже подавали заявку на эту стажировку.");
                return View(vm);
            }

            var application = new Application
            {
                InternshipId = vm.InternshipId,
                StudentId = student.Id,
                MotivationLetter = vm.MotivationLetter,
                HasPortfolio = vm.HasPortfolio,
                ApplicationDate = DateTime.UtcNow,
                Status = ApplicationStatus.Pending
            };

            _applicationRepo.Add(application);
            return RedirectToAction(nameof(Index));
        }

        // Список заявок по компании с поиском, сортировкой и пагинацией
        [Authorize(Roles = "COMPANY")]
        public async Task<IActionResult> CompanyApplications(string searchStudent, string sortOrder, int pageNumber = 1)
        {
            int pageSize = 5;

            var user = await _userManager.GetUserAsync(User);
            var company = await _internshipRepo.GetAllQueryable()
                .Include(i => i.Company)
                .Where(i => i.Company.ContactEmail == user.Email)
                .Select(i => i.Company)
                .FirstOrDefaultAsync();

            if (company == null)
                return Forbid();

            var internships = _internshipRepo.GetAllQueryable()
                .Where(i => i.CompanyId == company.Id);

            var query = internships
                .SelectMany(i => i.Applications)
                .Include(a => a.Student)
                .Include(a => a.Internship)
                .AsQueryable();

            // Поиск по имени студента
            if (!string.IsNullOrWhiteSpace(searchStudent))
            {
                query = query.Where(a => a.Student.FullName.Contains(searchStudent));
            }

            // Сортировка
            ViewData["CurrentSort"] = sortOrder;
            ViewData["StudentSortParm"] = sortOrder == "Student" ? "student_desc" : "Student";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            query = sortOrder switch
            {
                "Student" => query.OrderBy(a => a.Student.FullName),
                "student_desc" => query.OrderByDescending(a => a.Student.FullName),
                "Status" => query.OrderBy(a => a.Status),
                "status_desc" => query.OrderByDescending(a => a.Status),
                "Date" => query.OrderBy(a => a.ApplicationDate),
                "date_desc" => query.OrderByDescending(a => a.ApplicationDate),
                _ => query.OrderByDescending(a => a.ApplicationDate)
            };

            var pagedList = await PaginatedList<Application>.CreateAsync(query, pageNumber, pageSize);

            var vmList = pagedList.Select(a => new ApplicationVM
            {
                Id = a.Id,
                StudentName = a.Student?.FullName,
                InternshipTitle = a.Internship?.Title,
                Status = a.Status,
                StudentRegistrationDate = a.Student?.RegistrationDate ?? DateTime.MinValue
            }).ToList();

            var vm = new CompanyApplicationsVM
            {
                Applications = vmList,
                Pagination = pagedList,
                CurrentFilter = searchStudent,
                CurrentSort = sortOrder
            };

            return View(vm);
        }

        // Обновление статуса заявки (для компании)
        [HttpPost]
        [Authorize(Roles = "COMPANY")]
        public async Task<IActionResult> UpdateStatus(int applicationId, ApplicationStatus status)
        {
            var application = await _applicationRepo.GetAllQueryable()
                .Include(a => a.Internship)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
                return NotFound();

            application.Status = status;
            _applicationRepo.Update(application);

            return RedirectToAction(nameof(CompanyApplications));
        }
    }
}
