using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            this._studentRepository = studentRepository;
        }

        public virtual List<Student> GetAllStudents()
        {
            return this._studentRepository.GetAll();
        }

        public virtual Student GetStudentById(int studentId)
        {
            return this._studentRepository.GetById(studentId);
        }

        public virtual void UpdateStudent(Student student)
        {
            this._studentRepository.Update(student);
        }

        public virtual void DeleteStudent(int id)
        {
            this._studentRepository.Remove(id);
        }

        public virtual Student CreateStudent(Student student)
        {
            return this._studentRepository.Create(student);
        }
    }
}
