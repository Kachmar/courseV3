using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Models.Models;
using Services;
using WebApi.Dto;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentService _studentService;

        public StudentController(StudentService studentService)
        {
            _studentService = studentService;
        }

        // GET: api/Student
        [HttpGet]
        public ActionResult<IEnumerable<StudentDto>> Get()
        {
            return Ok(_studentService.GetAllStudents().Select(student => StudentDto.FromModel(student)));
        }

        // GET api/Student/5
        [HttpGet("{id}")]
        public ActionResult<StudentDto> Get(int id)
        {
            var student = _studentService.GetStudentById(id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(StudentDto.FromModel(student));
        }

        // POST api/Student
        [HttpPost]
        public ActionResult<StudentDto> Post([FromBody] StudentDto student)
        {
            var createdStudent = _studentService.CreateStudent(student.ToModel());
            return Accepted(StudentDto.FromModel(createdStudent));
        }

        // PUT api/Student/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] StudentDto value)
        {
            _studentService.UpdateStudent(value.ToModel());
            return Accepted();
        }

        // DELETE api/Student/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _studentService.DeleteStudent(id);
            return Accepted();
        }
    }
}
