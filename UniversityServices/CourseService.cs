using System;
using System.Collections.Generic;
using System.Linq;
using Models;
using Models.Models;
using Services.Abstraction;
using Services.Validators;

namespace Services
{
    public class CourseService
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Student> _studentRepository;

        public CourseService()
        {

        }

        public CourseService(IRepository<Course> courseRepository, IRepository<Student> studentRepository)
        {
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
        }

        public virtual List<Course> GetAllCourses()
        {
            return _courseRepository.GetAll();
        }

        public virtual void DeleteCourse(int id)
        {
            this._courseRepository.Remove(id);
        }

        public virtual Course GetCourseById(int id)
        {
            return this._courseRepository.GetById(id);
        }

        public virtual ValidationResponse UpdateCourse(Course course)
        {
            ValidationResponse<Course> response = ValidateCourse(course);
            if (response.HasErrors)
            {
                return response;
            }
            this._courseRepository.Update(course);
            return new ValidationResponse();
        }

        public virtual ValidationResponse<Course> CreateCourse(Course course)
        {
            ValidationResponse<Course> response = ValidateCourse(course);
            if (response.HasErrors)
            {
                return response;
            }

            var all = this._courseRepository.GetAll();
            if (all.Any(p => p.Name == course.Name))
            {
                return new ValidationResponse<Course>("name", $"course with name '{course.Name}' already exists.");
            }
            var newCourse = this._courseRepository.Create(course);
            return new ValidationResponse<Course>(newCourse);
        }

        public virtual void SetStudentsToCourse(int courseId, IEnumerable<int> studentIds)
        {
            var course = this._courseRepository.GetById(courseId);
            course.Students.Clear();
            foreach (var studentId in studentIds)
            {
                var student = this._studentRepository.GetById(studentId);
                course.Students.Add(student);
            }
            this._courseRepository.Update(course);
        }

        private ValidationResponse<Course> ValidateCourse(Course course)
        {
            if (course == null)
            {
                return new ValidationResponse<Course>("course", "Course cannot be null");
            }

            if (course.StartDate > course.EndDate)
            {
                return new ValidationResponse<Course>(nameof(course.StartDate), "Start date cannot be greater than end date!");
            }

            return new ValidationResponse<Course>(course);
        }
    }
}
