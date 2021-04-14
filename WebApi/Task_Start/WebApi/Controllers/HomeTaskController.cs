using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Services;
using WebApi.Dto;

namespace WebApi.Controllers // доробити !!!
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeTaskController : ControllerBase
    {
        private readonly HomeTaskService _hometaskService;

        public HomeTaskController(HomeTaskService hometaskService)
        {
            _hometaskService = hometaskService;
        }
        // GET: api/Course
        [HttpGet]
        public ActionResult<IEnumerable<HomeTaskDto>> Get()
        {
            return Ok(_hometaskService.GetAllHomeTasks().Select(homeTask => HomeTaskDto.FromModel(homeTask)));
        }

        // GET api/Course/5
        [HttpGet("{id}")]
        public ActionResult<HomeTaskDto> Get(int id)
        {
            var homeTask = _hometaskService.GetHomeTaskById(id);

            if (homeTask == null)
            {
                return NotFound();
            }

            return Ok(HomeTaskDto.FromModel(homeTask));
        }

        // PUT api/Course/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] HomeTaskDto value)
        {
            var updateResult = _hometaskService.UpdateHomeTask(value.ToModel());
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
            _hometaskService.DeleteHomeTask(id);
            return Accepted();
        }
    }
}
