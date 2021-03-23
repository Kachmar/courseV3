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
    public class CourseServiceTest
    {
        private const int HomeTaskId = 23;
        private const int HomeTaskAssessmentId = 55;

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
        public void CreateCourse_ReturnsValidationError_WhenNullPassed()
        {
            //Arrange
            CourseService courseService = new CourseService(null, null, null, null);

            //Act
            var createdCourse = courseService.CreateCourse(null);

            //Assert
            ValidationResponse<Course> expected = new ValidationResponse<Course>("course", "Course cannot be null");
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
            courseRepository.GetAll().Returns(new List<Course>());

            CourseService courseService = new CourseService(courseRepository, null, null, null);

            //Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => { courseService.SetStudentsToCourse(5, new List<int>() { 3 }); });
            Assert.Equal("There is no course with id '5'", exception.Message);
        }

        [Fact]
        public void SetStudentsToCourse_NoStudentExistsWithGivenId_ThrowsException()
        {
            //Arrange
            var courseRepository = Substitute.For<IRepository<Course>>();
            var course = GetDefaultCourse(10);
            course.Students.Add(new Student() { Id = 2 });
            courseRepository.GetById(course.Id).Returns(course);
            var studentRepository = Substitute.For<IRepository<Student>>();

            CourseService courseService = new CourseService(courseRepository, studentRepository, null, null);

            //Act and Assert
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                courseService.SetStudentsToCourse(10, new List<int>()
                {
                    3, 4
                });
            });
            Assert.Equal("Cannot find student with id '3'", exception.Message);
        }

        [Fact]
        public void UpdateCourse_ReturnEmptyValidationResponse_WhenCourseIsValid()
        {
            //Arrange
            Course course = GetDefaultCourse();
            var courseRepository = Substitute.For<IRepository<Course>>();
            courseRepository.GetAll().Returns(new List<Course>());
            CourseService courseService = new CourseService(courseRepository, null, null, null);

            //Act
            var result = courseService.UpdateCourse(course);

            //Assert
            ValidationResponse expected = new ValidationResponse();
            expected.Should().BeEquivalentTo(result);
            courseRepository.Received(Quantity.Exactly(1)).Update(course);
        }

        [Fact]
        public void DeleteCourse_RemovesCourse_WhenCourseExist()
        {
            //Arrange
            HomeTaskAssessment homeTaskAssessment = new HomeTaskAssessment()
            {
                Id = HomeTaskAssessmentId
            };
            HomeTask homeTask = new HomeTask()
            {
                HomeTaskAssessments = new List<HomeTaskAssessment>()
            {
                homeTaskAssessment
            },
                Id = HomeTaskId
            };
            Course course = GetDefaultCourse();
            course.HomeTasks = new List<HomeTask>()
            {
                homeTask
            };
            var courseRepository = Substitute.For<IRepository<Course>>();
            courseRepository.GetById(course.Id).Returns(course);
            IRepository<HomeTask> homeTaskRepository = Substitute.For<IRepository<HomeTask>>();
            IRepository<HomeTaskAssessment> homeTaskAssessmentRepository = Substitute.For<IRepository<HomeTaskAssessment>>();
            CourseService courseService = new CourseService(courseRepository, null, homeTaskRepository, homeTaskAssessmentRepository);

            //Act
            courseService.DeleteCourse(course.Id);

            //Assert
            courseRepository.Received(Quantity.Exactly(1)).Remove(course.Id);
            homeTaskRepository.Received(Quantity.Exactly(1)).Remove(HomeTaskId);
            homeTaskAssessmentRepository.Received(Quantity.Exactly(1)).Remove(HomeTaskAssessmentId);
        }


        [Fact]
        public void DeleteCourse_ThrowsException_WhenNoCourseExist()
        {
            //Arrange
            var courseRepository = Substitute.For<IRepository<Course>>();
            CourseService courseService = new CourseService(courseRepository, null, null, null);

            //Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => { courseService.DeleteCourse(5); });
            Assert.Equal("Cannot find course with id '5'", exception.Message);
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
