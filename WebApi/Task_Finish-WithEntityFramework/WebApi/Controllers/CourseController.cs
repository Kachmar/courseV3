using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Services;
using WebApi.Dto;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly CourseService _courseService;

        public CourseController(CourseService courseService)
        {
            _courseService = courseService;
        }
        // GET: api/Course
        [HttpGet]
        public ActionResult<IEnumerable<CourseDto>> Get()
        {
            return Ok(_courseService.GetAllCourses().Select(course => CourseDto.FromModel(course)));
        }

        // GET api/Course/5
        [HttpGet("{id}")]
        public ActionResult<CourseDto> Get(int id)
        {
            var course = _courseService.GetCourseById(id);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(CourseDto.FromModel(course));
        }

        // POST api/Course
        [HttpPost]
        public ActionResult<CourseDto> Post([FromBody] CourseDto course)
        {
            var createResult = _courseService.CreateCourse(course.ToModel());
            if (createResult.HasErrors)
            {
                return BadRequest(createResult.Errors);
            }
            return Accepted(CourseDto.FromModel(createResult.Result));
        }

        // PUT api/Course/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] CourseDto value)
        {
            var updateResult = _courseService.UpdateCourse(value.ToModel());
            if (updateResult.HasErrors)
            {
                return BadRequest(updateResult.Errors);
            }
            return Accepted();
        }

        // DELETE api/Course/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _courseService.DeleteCourse(id);
            return Accepted();
        }
    }
}
