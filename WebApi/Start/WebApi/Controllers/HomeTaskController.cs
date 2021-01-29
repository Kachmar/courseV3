using Microsoft.AspNetCore.Http;
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
            var createdHomeTask = _homeTaskService.CreateHomeTask(homeTask.ToModel());
            return Accepted(HomeTaskDto.FromModel(createdHomeTask));
        }

        // PUT api/HomeTask/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] HomeTaskDto value)
        {
            _homeTaskService.UpdateHomeTask(value.ToModel());
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
