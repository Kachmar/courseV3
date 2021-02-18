using System;

namespace University.MVC.ViewModels
{
    public class HomeTaskViewModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Number { get; set; }

        public int CourseId { get; set; }
    }
}