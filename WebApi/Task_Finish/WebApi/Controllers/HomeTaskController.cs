using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Services;
using WebApi.Dto;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeTaskController : ControllerBase
    {
        private readonly HomeTaskService _homeTaskService;

        public HomeTaskController(HomeTaskService homeTaskService)
        {
            _homeTaskService = homeTaskService;
        }

        // GET: api/HomeTask
        [HttpGet]
        public ActionResult<IEnumerable<HomeTaskDto>> Get()
        {
            return Ok(_homeTaskService.GetAllHomeTasks().Select(homeTask => HomeTaskDto.FromModel(homeTask)));
        }

        // GET api/HomeTask/5
        [HttpGet("{id}")]
        public ActionResult<HomeTaskDto> Get(int id)
        {
            var homeTask = _homeTaskService.GetHomeTaskById(id);

            if (homeTask == null)
            {
                return NotFound();
            }

            return Ok(HomeTaskDto.FromModel(homeTask));
        }

        // POST api/HomeTask
        [HttpPost]
        public ActionResult<HomeTaskDto> Post([FromBody] HomeTaskDto homeTask)
        {
            var result = _homeTaskService.CreateHomeTask(homeTask.ToModel());
            if (result.HasErrors)
            {
                return BadRequest(result.Errors);
            }
            return Accepted(HomeTaskDto.FromModel(result.Result));
        }

        // PUT api/HomeTask/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] HomeTaskDto value)
        {
            var result = _homeTaskService.UpdateHomeTask(value.ToModel());
            if (result.HasErrors)
            {
                return BadRequest(result.Errors);
            }
            return Accepted();
        }

        // DELETE api/HomeTask/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _homeTaskService.DeleteHomeTask(id);
            return Accepted();
        }
    }
}
