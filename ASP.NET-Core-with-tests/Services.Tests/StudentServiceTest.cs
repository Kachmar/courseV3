using System;
using System.Collections.Generic;
using FluentAssertions;
using Models;
using Models.Models;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Services.Validators;
using Xunit;

namespace Services.Tests
{
    public class StudentServiceTest
    {
        private const int HomeTaskAssessmentId = 55;

        [Fact]
        public void CreateStudent_ReturnsInsertedStudent()
        {
            //Arrange
            Student studentToInsert = GetDefaultStudent();
            var studentRepository = Substitute.For<IRepository<Student>>();
            var insertedStudent = GetDefaultStudent(5);
            studentRepository.Create(studentToInsert).Returns(insertedStudent);
            studentRepository.GetAll().Returns(new List<Student>());
            StudentService studentService = new StudentService(studentRepository, null);

            //Act
            var createdStudent = studentService.CreateStudent(studentToInsert);

            //Assert
            Student expectedStudent = GetDefaultStudent(5);
            ValidationResponse<Student> expected = new ValidationResponse<Student>(expectedStudent);
            expected.Should().BeEquivalentTo(createdStudent);
        }

        [Fact]
        public void CreateStudent_ReturnsValidationError_WhenExistsWithSameEmail()
        {
            //Arrange
            Student studentToInsert = GetDefaultStudent();
            var studentRepository = Substitute.For<IRepository<Student>>();
            studentRepository.GetAll().Returns(new List<Student>() { studentToInsert });
            StudentService studentService = new StudentService(studentRepository, null);

            //Act
            var createdStudent = studentService.CreateStudent(studentToInsert);

            //Assert
            ValidationResponse<Student> expected = new ValidationResponse<Student>("email", "Student with email 'test@gmail.com' already exists.");
            expected.Should().BeEquivalentTo(createdStudent);
        }

        [Fact]
        public void CreateStudent_ReturnsValidationError_WhenNullPassed()
        {
            //Arrange
            StudentService studentService = new StudentService(null, null);

            //Act
            var result = studentService.CreateStudent(null);

            //Assert
            ValidationResponse<Student> expected = new ValidationResponse<Student>("student", "Student cannot be null");
            expected.Should().BeEquivalentTo(result);
        }

        [Fact]
        public void UpdateStudent_ReturnEmptyValidationResponse_WhenStudentIsValid()
        {
            //Arrange
            Student student = GetDefaultStudent();
            var studentRepository = Substitute.For<IRepository<Student>>();
            studentRepository.GetAll().Returns(new List<Student>());
            StudentService studentService = new StudentService(studentRepository, null);

            //Act
            var result = studentService.UpdateStudent(student);

            //Assert
            ValidationResponse expected = new ValidationResponse();
            expected.Should().BeEquivalentTo(result);
            studentRepository.Received(Quantity.Exactly(1)).Update(student);
        }

        [Fact]
        public void DeleteStudent_RemovesStudent_WhenStudentExist()
        {
            //Arrange
            HomeTaskAssessment homeTaskAssessment = new HomeTaskAssessment()
            {
                Id = HomeTaskAssessmentId
            };

            Student student = GetDefaultStudent();
            student.HomeTaskAssessments = new List<HomeTaskAssessment>()
            {
                homeTaskAssessment
            };
            var studentRepository = Substitute.For<IRepository<Student>>();
            studentRepository.GetAll().Returns(new List<Student>());
            studentRepository.GetById(student.Id).Returns(student);
            IRepository<HomeTaskAssessment> homeTaskAssessmentRepository = Substitute.For<IRepository<HomeTaskAssessment>>();
            StudentService studentService = new StudentService(studentRepository, homeTaskAssessmentRepository);

            //Act
            studentService.DeleteStudent(student.Id);

            //Assert
            homeTaskAssessmentRepository.Received(Quantity.Exactly(1)).Remove(HomeTaskAssessmentId);
            studentRepository.Received(Quantity.Exactly(1)).Remove(student.Id);
        }

        [Fact]
        public void DeleteStudent_ThrowsException_WhenNoStudentExist()
        {
            //Arrange
            var studentRepository = Substitute.For<IRepository<Student>>();
            StudentService studentService = new StudentService(studentRepository, null);

            //Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => { studentService.DeleteStudent(5); });
            Assert.Equal("Cannot find student with id '5'", exception.Message);
        }

        private Student GetDefaultStudent(int id = 0)
        {
            return new Student()
            {
                Name = "Test",
                Email = "test@gmail.com",
                Id = id
            };
        }
    }
}