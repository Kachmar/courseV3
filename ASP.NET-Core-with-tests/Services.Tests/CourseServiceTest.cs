using System;
using System.Collections.Generic;
using FluentAssertions;
using Models;
using Models.Models;
using NSubstitute;
using Services.Validators;
using Xunit;

namespace Services.Tests
{
    public class CourseServiceTest
    {
        [Fact]
        public void CreateCourse_ReturnsInsertedCourse()
        {
            //Arrange
            Course courseToInsert = GetDefaultCourse();
            var courseRepository = Substitute.For<IRepository<Course>>();
            var insertedCourse = GetDefaultCourse(5);
            courseRepository.Create(courseToInsert).Returns(insertedCourse);
            courseRepository.GetAll().Returns(new List<Course>());
            CourseService courseService = new CourseService(courseRepository, null, null, null);

            //Act
            var createdCourse = courseService.CreateCourse(courseToInsert);

            //Assert
            Course expectedCourse = GetDefaultCourse(5);
            ValidationResponse<Course> expected = new ValidationResponse<Course>(expectedCourse);
            expected.Should().BeEquivalentTo(createdCourse);
        }

        [Fact]
        public void CreateCourse_ReturnsValidationError_WhenStartDateGreaterThanEndDate()
        {
            //Arrange
            Course courseToInsert = GetDefaultCourse();
            courseToInsert.StartDate = new DateTime(2024, 1, 1);
            CourseService courseService = new CourseService(null, null, null, null);

            //Act
            var createdCourse = courseService.CreateCourse(courseToInsert);

            //Assert
            ValidationResponse<Course> expected = new ValidationResponse<Course>(nameof(courseToInsert.StartDate),
                "Start date cannot be greater than end date!");
            expected.Should().BeEquivalentTo(createdCourse);
        }

        [Fact]
        public void CreateCourse_ReturnsValidationError_WhenExistsWithSameName()
        {
            //Arrange
            Course courseToInsert = GetDefaultCourse();
            var courseRepository = Substitute.For<IRepository<Course>>();
            courseRepository.GetAll().Returns(new List<Course>() { courseToInsert });
            CourseService courseService = new CourseService(courseRepository, null, null, null);

            //Act
            var createdCourse = courseService.CreateCourse(courseToInsert);

            //Assert
            ValidationResponse<Course> expected = new ValidationResponse<Course>("name", "course with name 'Test' already exists.");
            expected.Should().BeEquivalentTo(createdCourse);
        }

        [Fact]
        public void SetStudentsToCourse_PositiveCase()
        {
            //Arrange
            var courseRepository = Substitute.For<IRepository<Course>>();
            var course = GetDefaultCourse(10);
            course.Students.Add(new Student() { Id = 2 });
            courseRepository.GetById(course.Id).Returns(course);
            var studentRepository = Substitute.For<IRepository<Student>>();

            studentRepository.GetById(3).Returns(GetDefaultStudent(3));
            studentRepository.GetById(4).Returns(GetDefaultStudent(4));
            CourseService courseService = new CourseService(courseRepository, studentRepository, null, null);
            Course actual = null;
            courseRepository
                .WhenForAnyArgs(a => a.Update(null))
                .Do(a => actual = a.Arg<Course>());
            //Act
            courseService.SetStudentsToCourse(10, new List<int>()
            {
                3, 4
            });

            //Assert
            var expectedCourse = GetDefaultCourse(10);
            expectedCourse.Students.Add(GetDefaultStudent(3));
            expectedCourse.Students.Add(GetDefaultStudent(4));

            actual.Should().BeEquivalentTo(expectedCourse);
        }

        [Fact]
        public void SetStudentsToCourse_NoCourseExistWithGivenId_ThrowsException()
        {
            //Arrange
            var courseRepository = Substitute.For<IRepository<Course>>();

            //courseRepository.GetById(Arg.Any<int>()).Returns(nu);
            CourseService courseService = new CourseService(courseRepository, null, null, null);

            //Act and Assert
            Assert.Throws<ArgumentException>(() => { courseService.SetStudentsToCourse(5, new List<int>() { 3 }); });
        }


        [Fact]
        public void SetStudentsToCourse_NoStudentExistsWithGivenId_ThrowsException()
        {

        }

        private Course GetDefaultCourse(int id = 0)
        {
            return new Course()
            {
                Id = id,
                EndDate = new DateTime(2020, 2, 2),
                StartDate = new DateTime(2020, 1, 1),
                Name = "Test",
                PassCredits = 10
            };
        }
        private Student GetDefaultStudent(int id = 0)
        {
            return new Student()
            {
                Name = "Test",
                Id = id
            };
        }
    }
}
