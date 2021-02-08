using System.Collections.Generic;
using Models;
using Models.Models;

namespace Services
{
    public class HomeTaskService
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<HomeTask> _homeTaskRepository;

        public HomeTaskService()
        {
            
        }

        public HomeTaskService(IRepository<Course> courseRepository, IRepository<HomeTask> homeTaskRepository)
        {
            this._courseRepository = courseRepository;
            this._homeTaskRepository = homeTaskRepository;
        }

        public virtual HomeTask CreateHomeTask(HomeTask homeTask)
        {
            //Todo think if it is needed to retrieve course
            var course = _courseRepository.GetById(homeTask.CourseId);
            homeTask.Course = course;
            return this._homeTaskRepository.Create(homeTask);
        }

        public virtual HomeTask GetHomeTaskById(int id)
        {
            return this._homeTaskRepository.GetById(id);
        }

        public virtual void UpdateHomeTask(HomeTask homeTask)
        {
            this._homeTaskRepository.Update(homeTask);
        }

        public virtual void DeleteHomeTask(int homeTaskId)
        {
            this._homeTaskRepository.Remove(homeTaskId);
        }

        public virtual List<HomeTask> GetAllHomeTasks()
        {
            return _homeTaskRepository.GetAll();
        }
    }
}
