using System;
using System.Collections.Generic;

namespace University.MVC.ViewModels
{
    public class CourseViewModel
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int PassCredits { get; set; }

        public List<HomeTaskViewModel> HomeTasks { get; set; }
    }
}