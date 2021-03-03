using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Models.Models;

namespace ADO.NET
{
    public class StudentRepository : RepositoryBase, IRepository<Student>
    {
        public StudentRepository(string connectionString) : base(connectionString)
        {
        }

        internal List<Student> GetAll(bool loadChildEntities)
        {
            using SqlConnection connection = GetConnection();
            SqlCommand sqlCommand = new SqlCommand(
                "select Id, Name, BirthDate, PhoneNumber, Email, GitHubLink, Notes from Students",
                connection);
            List<Student> students = new List<Student>();
            using (var reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    Student student = new Student();
                    student.Id = reader.GetInt32(0);
                    student.Name = reader.GetStringOrDefault(1);
                    student.BirthDate = reader.GetDateTime(2);
                    student.PhoneNumber = reader.GetStringOrDefault(3);
                    student.Email = reader.GetStringOrDefault(4);
                    student.GitHubLink = reader.GetStringOrDefault(5);
                    student.Notes = reader.GetStringOrDefault(6);
                    if (loadChildEntities)
                    {
                        student.Courses = GetStudentCourses(student.Id);
                        student.HomeTaskAssessments = GetHomeTaskAssessments(student.Id);
                    }

                    students.Add(student);
                }
            }


            return students;
        }

        private List<HomeTaskAssessment> GetHomeTaskAssessments(int studentId)
        {
            List<HomeTaskAssessment> result = new List<HomeTaskAssessment>();
            using (SqlConnection connection = GetConnection())
            {
                SqlCommand sqlCommand = new SqlCommand(
                    $@"
                   SELECT [Id]                 
                  ,[IsComplete]
                  ,[Date]                              
              FROM [dbo].[HomeTaskAssessment]             
              where StudentId =  {studentId}", connection);

                using var reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    HomeTaskAssessment homeTaskAssessment = new HomeTaskAssessment();
                    homeTaskAssessment.Id = reader.GetInt32(0);
                    homeTaskAssessment.IsComplete = reader.GetBoolean(1);
                    homeTaskAssessment.Date = reader.GetDateTime(2);
                    result.Add(homeTaskAssessment);
                }
            }

            return result;
        }

        private List<Course> GetStudentCourses(int studentId)
        {
            List<Course> result = new List<Course>();
            using (SqlConnection connection = GetConnection())
            {
                SqlCommand sqlCommand = new SqlCommand(
                    $@"
                   SELECT [Id]
                  ,[Name]
                  ,[StartDate]
                  ,[EndDate]
                  ,[PassCredits]               
              FROM [dbo].[Courses] as c
              join CourseStudent as sc on sc.CoursesId=c.Id
              where sc.StudentsId =  {studentId}", connection);

                using var reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Course course = new Course();
                    course.Id = reader.GetInt32(0);
                    course.Name = reader.GetStringOrDefault(1);
                    course.StartDate = reader.GetDateTime(2);
                    course.EndDate = reader.GetDateTime(3);
                    course.PassCredits = reader.GetInt32(4);
                    result.Add(course);
                }
            }

            return result;
        }

        public List<Student> GetAll()
        {
            return this.GetAll(true);

        }

        public Student GetById(int id)
        {
            return this.GetAll().SingleOrDefault(student => student.Id == id);
        }

        public Student Create(Student student)
        {
            throw new NotImplementedException();
        }

        public void Update(Student student)
        {
            throw new NotImplementedException();

        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        private static void SetStudentToCourses(IEnumerable<int> coursesId, int studentId, SqlTransaction transaction)
        {
            SqlCommand sqlCommand = new SqlCommand($@"DELETE FROM [dbo].[CourseStudent]
            WHERE StudentsId = {studentId}", transaction.Connection, transaction);
            sqlCommand.ExecuteNonQuery();
            foreach (var courseId in coursesId)
            {
                sqlCommand = new SqlCommand(
                    $@"INSERT INTO [dbo].[CourseStudent]
           ([CoursesId]
           ,[StudentsId])
            VALUES
           ({courseId},{studentId})",
                    transaction.Connection,
                    transaction);

                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}
