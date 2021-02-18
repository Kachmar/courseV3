using Models.Models;
using System.Collections.Generic;
using Models;

namespace Services
{
    public class StudentService
    {
        private readonly IRepository<Student> _studentRepository;

        public StudentService()
        {
            
        }

        public StudentService(IRepository<Student> studentRepository)
        {
            _studentRepository = studentRepository;
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
            _studentRepository.Remove(id);
        }

        public virtual Student CreateStudent(Student student)
        {
            return _studentRepository.Create(student);
        }
    }
}
