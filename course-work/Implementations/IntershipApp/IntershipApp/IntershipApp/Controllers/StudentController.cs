using Microsoft.AspNetCore.Mvc;
using Common.Entities;
using Common.Repositories;
using InternshipApp.ViewModels;

namespace InternshipApp.Controllers
{
    public class StudentController : Controller
    {
        private readonly BaseRepository<Student> _studentRepo = new BaseRepository<Student>();

        public IActionResult Index()
        {
            var students = _studentRepo.GetAll();

            var viewModelList = students.Select(s => new StudentVM
            {
                Id = s.Id,
                FullName = s.FullName,
                Email = s.Email,
                University = s.University,
                Degree = s.Degree
            }).ToList();

            return View(viewModelList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new StudentVM()); // обязательно передаём модель
        }

        [HttpPost]
        public IActionResult Create(StudentVM studentVM)
        {
            if (!ModelState.IsValid)
            {
                return View(studentVM);
            }

            var student = new Student
            {
                FullName = studentVM.FullName,
                Email = studentVM.Email,
                University = studentVM.University,
                Degree = studentVM.Degree
            };

            _studentRepo.Add(student);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var student = _studentRepo.GetById(id);
            if (student == null)
                return NotFound();

            var vm = new StudentVM
            {
                Id = student.Id,
                FullName = student.FullName,
                Email = student.Email,
                University = student.University,
                Degree = student.Degree
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(StudentVM studentVM)
        {
            if (!ModelState.IsValid)
                return View(studentVM);

            var student = _studentRepo.GetById(studentVM.Id);
            if (student == null)
                return NotFound();

            student.FullName = studentVM.FullName;
            student.Email = studentVM.Email;
            student.University = studentVM.University;
            student.Degree = studentVM.Degree;

            _studentRepo.Update(student);

            return RedirectToAction(nameof(Index));
        }

        // Метод для отображения подтверждения удаления
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var student = _studentRepo.GetById(id);
            if (student == null)
                return NotFound();

            return View(student); // Передаем модель на представление
        }

        // Метод для удаления студента
        [HttpPost]
        [ActionName("Delete")] // Это указывает на метод Delete, но для POST-запроса
        public IActionResult DeleteConfirmed(int id)
        {
            var student = _studentRepo.GetById(id);
            if (student == null)
                return NotFound();

            _studentRepo.Delete(student); // Удаляем студента

            return RedirectToAction(nameof(Index)); // Перенаправляем на главную страницу
        }

    }
}

