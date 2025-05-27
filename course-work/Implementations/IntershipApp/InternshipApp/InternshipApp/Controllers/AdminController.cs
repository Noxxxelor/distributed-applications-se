using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Common.Models;
using Common.Entities;
using Microsoft.EntityFrameworkCore;
using InternshipApp.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Common.Repositories;
using InternshipApp.Helpers;

namespace InternshipApp.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(
    string userSortOrder = null,
    string userSearchString = null,
    int userPage = 1,
    string studentSortOrder = null,
    string studentSearchString = null,
    int studentPage = 1,
    string companySortOrder = null,
    string companySearchString = null,
    int companyPage = 1)
        {
            int pageSize = 10;
            var currentUserId = _userManager.GetUserId(User);

            // --- USERS ---
            var userQuery = _userManager.Users.Where(u => u.Id != currentUserId);

            if (!string.IsNullOrEmpty(userSearchString))
            {
                userQuery = userQuery.Where(u => u.UserName.Contains(userSearchString) || u.Email.Contains(userSearchString));
            }

            userQuery = userSortOrder switch
            {
                "username_desc" => userQuery.OrderByDescending(u => u.UserName),
                "email" => userQuery.OrderBy(u => u.Email),
                "email_desc" => userQuery.OrderByDescending(u => u.Email),
                _ => userQuery.OrderBy(u => u.UserName)
            };

            var users = await PaginatedList<ApplicationUser>.CreateAsync(userQuery, userPage, pageSize);

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.Role = roles.FirstOrDefault() ?? "";
            }

            // --- STUDENTS ---
            var studentQuery = _context.Students.AsNoTracking();

            if (!string.IsNullOrEmpty(studentSearchString))
            {
                studentQuery = studentQuery.Where(s => s.FullName.Contains(studentSearchString) || s.Email.Contains(studentSearchString));
            }

            studentQuery = studentSortOrder switch
            {
                "name_desc" => studentQuery.OrderByDescending(s => s.FullName),
                "email" => studentQuery.OrderBy(s => s.Email),
                "email_desc" => studentQuery.OrderByDescending(s => s.Email),
                _ => studentQuery.OrderBy(s => s.FullName)
            };

            var students = await PaginatedList<Student>.CreateAsync(studentQuery, studentPage, pageSize);

            // --- COMPANIES ---
            var companyQuery = _context.Companies.AsNoTracking();

            if (!string.IsNullOrEmpty(companySearchString))
            {
                companyQuery = companyQuery.Where(c => c.Name.Contains(companySearchString) || c.ContactEmail.Contains(companySearchString));
            }

            companyQuery = companySortOrder switch
            {
                "name_desc" => companyQuery.OrderByDescending(c => c.Name),
                "email" => companyQuery.OrderBy(c => c.ContactEmail),
                "email_desc" => companyQuery.OrderByDescending(c => c.ContactEmail),
                _ => companyQuery.OrderBy(c => c.Name)
            };

            var companies = await PaginatedList<Company>.CreateAsync(companyQuery, companyPage, pageSize);

            var model = new AdminDashboardVM
            {
                Users = users,
                Students = students,
                Companies = companies,
                UserSortOrder = userSortOrder,
                UserSearchString = userSearchString,
                StudentSortOrder = studentSortOrder,
                StudentSearchString = studentSearchString,
                CompanySortOrder = companySortOrder,
                CompanySearchString = companySearchString
            };

            return View(model);
        }


        public async Task<IActionResult> DeleteUser(string id)
        {
            if (id == null) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
            {
                return BadRequest("Вы не можете удалить самого себя.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("STUDENT"))
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == user.Email);
                if (student != null)
                {
                    var applications = _context.Applications.Where(a => a.StudentId == student.Id);
                    _context.Applications.RemoveRange(applications);

                    _context.Students.Remove(student);
                }
            }

            if (roles.Contains("COMPANY"))
            {
                var company = await _context.Companies
                    .Include(c => c.Internships)
                    .ThenInclude(i => i.Applications)
                    .FirstOrDefaultAsync(c => c.ContactEmail == user.Email);

                if (company != null)
                {
                    foreach (var internship in company.Internships)
                    {
                        _context.Applications.RemoveRange(internship.Applications);
                    }

                    _context.Internships.RemoveRange(company.Internships);
                    _context.Companies.Remove(company);
                }
            }

            await _context.SaveChangesAsync();
            await _userManager.DeleteAsync(user);

            return RedirectToAction("Index");
        }

        
        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var model = new ApplicationUser
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Role = roles.FirstOrDefault() ?? ""
            };

            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, ApplicationUser model)
        {
            if (id != model.Id)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            user.Email = model.Email;
            user.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            
            var existingRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, existingRoles);
            if (!string.IsNullOrWhiteSpace(model.Role))
                await _userManager.AddToRoleAsync(user, model.Role);

            return RedirectToAction(nameof(Index)); 
        }


    }
}
