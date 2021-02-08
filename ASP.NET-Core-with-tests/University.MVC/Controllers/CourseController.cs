using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Services;
using University.MVC.ViewModels;

namespace University.MVC.Controllers
{
    public class CourseController : Controller
    {
        private readonly CourseService courseService;
        private readonly StudentService studentService;

        public CourseController(CourseService courseService, StudentService studentService)
        {
            this.courseService = courseService;
            this.studentService = studentService;
        }

        // GET
        public IActionResult Courses()
        {
            return View(this.courseService.GetAllCourses());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            this.courseService.DeleteCourse(id);

            return RedirectToAction("Courses");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["action"] = nameof(this.Create);
            return this.View("Edit", new Course());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            Course course = this.courseService.GetCourseById(id);
            if (course == null)
            {
                return this.NotFound();
            }

            ViewData["action"] = nameof(this.Edit);

            return this.View(course);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Course courseParameter)
        {
            if (courseParameter == null)
            {
                return this.BadRequest();
            }
            courseService.UpdateCourse(courseParameter);
            return this.RedirectToAction(nameof(Courses));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(Course courseParameter)
        {
            if (courseParameter == null)
            {
                return this.BadRequest();
            }
            ViewData["action"] = nameof(this.Create);
            if (!ModelState.IsValid)
            {
                return this.View("Edit", courseParameter);
            }

            try
            {
                this.courseService.CreateCourse(courseParameter);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Name", ex.Message);
                return this.View("Edit", courseParameter);

            }

            return this.RedirectToAction(nameof(Courses));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AssignStudents(int id)
        {
            var allStudents = this.studentService.GetAllStudents();
            var course = this.courseService.GetCourseById(id);
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
            model.Students = new List<StudentViewModel>();

            foreach (var student in allStudents)
            {
                bool isAssigned = course.Students.Any(p => p.Id == student.Id);
                model.Students.Add(new StudentViewModel() { StudentId = student.Id, StudentFullName = student.Name, IsAssigned = isAssigned });
            }

            return this.View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AssignStudents(CourseStudentAssignmentViewModel assignmentViewModel)
        {
            this.courseService.SetStudentsToCourse(assignmentViewModel.Id, assignmentViewModel.Students.Where(p => p.IsAssigned).Select(student => student.StudentId));

            return RedirectToAction("Courses");
        }
    }
}