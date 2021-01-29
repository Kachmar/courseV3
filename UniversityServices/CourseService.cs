using System;
using System.Collections.Generic;
using System.Linq;
using Models;
using Models.Models;

namespace Services
{
    public class CourseService
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Student> _studentRepository;

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

        public virtual void UpdateCourse(Course course)
        {
            this._courseRepository.Update(course);
        }

        public virtual Course CreateCourse(Course course)
        {
            var all = this._courseRepository.GetAll();
            if (all.Any(p => p.Name == course.Name))
            {
                throw new Exception($"course with name '{course.Name}' already exists.");
            }
            return this._courseRepository.Create(course);
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
    }
}
