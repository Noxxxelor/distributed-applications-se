using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using InternshipApp.ViewModels;
using Common.Entities;
using System.Threading.Tasks;
using System;
using Common.Models;
using Common.Repositories;

namespace InternshipApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("STUDENT"))
                    return RedirectToAction("Index", "Internships");
                else if (roles.Contains("COMPANY"))
                    return RedirectToAction("MyInternships", "Company");
                else
                    return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Неверный логин или пароль");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Role = model.Role
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                string roleUpper = model.Role.ToUpperInvariant();
                await _userManager.AddToRoleAsync(user, roleUpper);

                if (roleUpper == "STUDENT")
                {
                    var student = new Student
                    {
                        FullName = model.StudentFullName,
                        Email = model.Email,
                        University = model.University,
                        Degree = model.Degree,
                        RegistrationDate = DateTime.Now,
                        IsActive = true
                    };

                    _context.Students.Add(student);
                }
                else if (roleUpper == "COMPANY")
                {
                    var company = new Company
                    {
                        Name = model.CompanyName,
                        ContactEmail = model.Email,
                        HeadquartersAddress = model.CompanyAddress,
                        CompanyPhone = model.CompanyPhone,   
                        EstablishedDate = DateTime.Now,
                        IsVerified = false
                    };



                    _context.Companies.Add(company);
                }

                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(user, false);

                if (roleUpper == "STUDENT")
                    return RedirectToAction("Index", "Internships");
                else if (roleUpper == "COMPANY")
                    return RedirectToAction("MyInternships", "Company");
                else
                    return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
