using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using Services;
using University.MVC.ViewModels;

namespace University.MVC.Controllers
{
    public class CourseController : Controller
    {
        private readonly CourseService _courseService;
        private readonly StudentService _studentService;

        public CourseController(CourseService courseService, StudentService studentService)
        {
            _courseService = courseService;
            _studentService = studentService;
        }

        public IActionResult Courses()
        {
            var courses = _courseService.GetAllCourses().Select(ToViewModel);
            return View(courses);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            _courseService.DeleteCourse(id);

            return RedirectToAction("Courses");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["action"] = nameof(this.Create);
            return View("Edit", new CourseViewModel());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var course = _courseService.GetCourseById(id);
            if (course == null)
            {
                return NotFound();
            }

            ViewData["action"] = nameof(this.Edit);

            return View(ToViewModel(course));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(CourseViewModel courseParameter)
        {
            if (courseParameter == null)
            {
                return BadRequest();
            }
            var validationResult = _courseService.UpdateCourse(ToModel(courseParameter));
            if (validationResult.HasErrors)
            {
                foreach (var validationResultError in validationResult.Errors)
                {
                    ModelState.AddModelError(validationResultError.Key, validationResultError.Value);
                }
                ViewData["action"] = nameof(this.Edit);
                return View(courseParameter);
            }
            return RedirectToAction(nameof(Courses));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(CourseViewModel courseParameter)
        {
            if (courseParameter == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View("Edit", courseParameter);
            }

            var validationResult = _courseService.CreateCourse(ToModel(courseParameter));
            if (validationResult.HasErrors)
            {
                foreach (var validationResultError in validationResult.Errors)
                {
                    ModelState.AddModelError(validationResultError.Key, validationResultError.Value);
                }

                return View("Edit", courseParameter);
            }

            return RedirectToAction(nameof(Courses));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AssignStudents(int id)
        {
            var allStudents = _studentService.GetAllStudents();
            var course = _courseService.GetCourseById(id);
            if (course == null)
            {
                return BadRequest();
            }
            CourseStudentAssignmentViewModel model = new CourseStudentAssignmentViewModel();

            model.Id = id;
            model.EndDate = course.EndDate;
            model.Name = course.Name;
            model.StartDate = course.StartDate;
            model.PassCredits = course.PassCredits;
            model.Students = new List<AssignmentStudentViewModel>();

            foreach (var student in allStudents)
            {
                bool isAssigned = course.Students.Any(p => p.Id == student.Id);
                model.Students.Add(new AssignmentStudentViewModel() { StudentId = student.Id, StudentFullName = student.Name, IsAssigned = isAssigned });
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AssignStudents(CourseStudentAssignmentViewModel assignmentViewModel)
        {
            _courseService.SetStudentsToCourse(assignmentViewModel.Id, assignmentViewModel.Students.Where(p => p.IsAssigned).Select(student => student.StudentId));

            return RedirectToAction("Courses");
        }

        private Course ToModel(CourseViewModel course)
        {
            return new Course()
            {
                EndDate = course.EndDate,
                StartDate = course.StartDate,
                Id = course.Id,
                Name = course.Name,
                PassCredits = course.PassCredits,
                HomeTasks = course.HomeTasks.Select(HomeTaskController.ToModel).ToList()
            };
        }

        private CourseViewModel ToViewModel(Course course)
        {
            return new CourseViewModel()
            {
                EndDate = course.EndDate,
                StartDate = course.StartDate,
                Id = course.Id,
                Name = course.Name,
                PassCredits = course.PassCredits,
                HomeTasks = course.HomeTasks.Select(HomeTaskController.ToViewModel).ToList()
            };
        }
    }
}