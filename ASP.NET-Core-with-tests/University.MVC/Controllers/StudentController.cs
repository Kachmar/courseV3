
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Services;
using University.MVC.ViewModels;

namespace University.MVC.Controllers
{
    public class StudentController : Controller
    {
        private readonly StudentService _studentService;
        private readonly IAuthorizationService _authorizationService;

        public StudentController(StudentService studentService, IAuthorizationService authorizationService)
        {
            _studentService = studentService;
            _authorizationService = authorizationService;
        }

        // GET
        public IActionResult Students()
        {
            return View(_studentService.GetAllStudents().Select(ToViewModel));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var student = _studentService.GetStudentById(id);
            var result = await _authorizationService.AuthorizeAsync(User, student, "SameUserPolicy");
            if (result.Succeeded)
            {
                ViewData["Action"] = "Edit";
                return View(ToViewModel(student));
            }

            return Forbid();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(StudentViewModel student)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Action"] = "Edit";
                return View("Edit", student);
            }

            var result = await _authorizationService.AuthorizeAsync(User, student, "SameUserPolicy");
            if (result.Succeeded)
            {
                _studentService.UpdateStudent(ToModel(student));

                return RedirectToAction("Students");
            }
            return Forbid();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            _studentService.DeleteStudent(id);
            return RedirectToAction("Students");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Action"] = "Create";
            var student = new StudentViewModel();
            return View("Edit", student);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(StudentViewModel student)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Action"] = "Create";
                return View("Edit", student);
            }

            _studentService.CreateStudent(ToModel(student));
            return RedirectToAction("Students");
        }

        private Student ToModel(StudentViewModel student)
        {
            return new Student()
            {
                BirthDate = student.BirthDate,
                Email = student.Email,
                GitHubLink = student.GitHubLink,
                Id = student.Id,
                Name = student.Name,
                Notes = student.Notes,
                PhoneNumber = student.PhoneNumber
            };
        }

        private StudentViewModel ToViewModel(Student student)
        {
            return new StudentViewModel()
            {
                BirthDate = student.BirthDate,
                Email = student.Email,
                GitHubLink = student.GitHubLink,
                Id = student.Id,
                Name = student.Name,
                Notes = student.Notes,
                PhoneNumber = student.PhoneNumber
            };
        }
    }
}