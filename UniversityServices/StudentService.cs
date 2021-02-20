using System;
using Models.Models;
using System.Collections.Generic;
using Models;

namespace Services
{
    public class StudentService
    {
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<HomeTaskAssessment> _homeTaskAssessmentRepository;

        public StudentService()
        {

        }

        public StudentService(IRepository<Student> studentRepository,
            IRepository<HomeTaskAssessment> homeTaskAssessmentRepository)
        {
            _studentRepository = studentRepository;
            _homeTaskAssessmentRepository = homeTaskAssessmentRepository;
        }

        public virtual List<Student> GetAllStudents()
        {
            return _studentRepository.GetAll();
        }

        public virtual Student GetStudentById(int studentId)
        {
            return _studentRepository.GetById(studentId);
        }

        public virtual void UpdateStudent(Student student)
        {
            _studentRepository.Update(student);
        }

        public virtual void DeleteStudent(int id)
        {
            var student = _studentRepository.GetById(id);
            if (student == null)
            {
                throw new Exception($"Cannot find student with id '{id}'");
            }

            foreach (var studentHomeTaskAssessment in student.HomeTaskAssessments.ToArray())
            {
                _homeTaskAssessmentRepository.Remove(studentHomeTaskAssessment.Id);
            }

            _studentRepository.Remove(id);
        }

        public virtual Student CreateStudent(Student student)
        {
            return _studentRepository.Create(student);
        }
    }
}
