using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Models.Models;
using Services;
using University.MVC.ViewModels;

namespace University.MVC.Controllers
{
    public class HomeTaskController : Controller
    {
        private readonly HomeTaskService _homeTaskService;
        private readonly StudentService _studentService;

        public HomeTaskController(HomeTaskService homeTaskService, StudentService studentService)
        {
            _homeTaskService = homeTaskService;
            _studentService = studentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(int courseId)
        {
            ViewData["Action"] = "Create";
            ViewData["CourseId"] = courseId;
            var model = new HomeTaskViewModel();
            return View("Edit", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(HomeTaskViewModel homeTask, int courseId)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Action"] = "Create";
                ViewData["CourseId"] = courseId;
                return View("Edit", homeTask);
            }
            var routeValueDictionary = new RouteValueDictionary();
            routeValueDictionary.Add("id", courseId);

            _homeTaskService.CreateHomeTask(ToModel(homeTask));
            return RedirectToAction("Edit", "Course", routeValueDictionary);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            HomeTask homeTask = _homeTaskService.GetHomeTaskById(id);
            if (homeTask == null)
                return NotFound();
            ViewData["Action"] = "Edit";

            return View(ToViewModel(homeTask));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(HomeTaskViewModel homeTaskParameter)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Action"] = "Edit";

                return View(homeTaskParameter);
            }

            var homeTask = _homeTaskService.GetHomeTaskById(homeTaskParameter.Id);

            var routeValueDictionary = new RouteValueDictionary();
            _homeTaskService.UpdateHomeTask(ToModel(homeTaskParameter));
            routeValueDictionary.Add("id", homeTask.Course.Id);
            return RedirectToAction("Edit", "Course", routeValueDictionary);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int homeTaskId, int courseId)
        {
            _homeTaskService.DeleteHomeTask(homeTaskId);

            var routeValueDictionary = new RouteValueDictionary();
            routeValueDictionary.Add("id", courseId);
            return RedirectToAction("Edit", "Course", routeValueDictionary);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Evaluate(int id)
        {
            var homeTask = _homeTaskService.GetHomeTaskById(id);

            if (homeTask == null)
            {
                return NotFound();
            }

            HomeTaskAssessmentViewModel assessmentViewModel =
                new HomeTaskAssessmentViewModel
                {
                    Date = homeTask.Date,
                    Description = homeTask.Description,
                    Title = homeTask.Title,
                    HomeTaskStudents = new List<HomeTaskStudentViewModel>(),
                    HomeTaskId = homeTask.Id
                };

            if (homeTask.HomeTaskAssessments.Any())
            {
                foreach (var homeTaskHomeTaskAssessment in homeTask.HomeTaskAssessments)
                {
                    assessmentViewModel.HomeTaskStudents.Add(new HomeTaskStudentViewModel()
                    {

                        StudentFullName = homeTaskHomeTaskAssessment.Student.Name,
                        StudentId = homeTaskHomeTaskAssessment.Student.Id,
                        IsComplete = homeTaskHomeTaskAssessment.IsComplete,
                        HomeTaskAssessmentId = homeTaskHomeTaskAssessment.Id
                    });
                }

            }
            else
            {
                foreach (var student in homeTask.Course.Students)
                {
                    assessmentViewModel.HomeTaskStudents.Add(new HomeTaskStudentViewModel() { StudentFullName = student.Name, StudentId = student.Id });
                }
            }

            return View(assessmentViewModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SaveEvaluation(HomeTaskAssessmentViewModel model)
        {
            var homeTask = _homeTaskService.GetHomeTaskById(model.HomeTaskId);

            if (homeTask == null)
            {
                return NotFound();
            }

            foreach (var homeTaskStudent in model.HomeTaskStudents)
            {
                var target = homeTask.HomeTaskAssessments.Find(p => p.Id == homeTaskStudent.HomeTaskAssessmentId);
                if (target != null)
                {
                    target.Date = DateTime.Now;
                    target.IsComplete = homeTaskStudent.IsComplete;
                }
                else
                {
                    var student = _studentService.GetStudentById(homeTaskStudent.StudentId);
                    homeTask.HomeTaskAssessments.Add(new HomeTaskAssessment
                    {
                        HomeTask = homeTask,
                        IsComplete = homeTaskStudent.IsComplete,
                        Student = student,
                        Date = DateTime.Now

                    });
                }
                _homeTaskService.UpdateHomeTask(homeTask);
            }
            return RedirectToAction("Courses", "Course");
        }

        public static HomeTaskViewModel ToViewModel(HomeTask homeTask)
        {
            return new HomeTaskViewModel()
            {
                CourseId = homeTask.CourseId,
                Id = homeTask.Id,
                Number = homeTask.Number,
                Date = homeTask.Date,
                Description = homeTask.Description,
                Title = homeTask.Title
            };
        }

        public static HomeTask ToModel(HomeTaskViewModel homeTask)
        {
            return new HomeTask()
            {
                CourseId = homeTask.CourseId,
                Id = homeTask.Id,
                Number = homeTask.Number,
                Date = homeTask.Date,
                Description = homeTask.Description,
                Title = homeTask.Title
            };
        }
    }
}