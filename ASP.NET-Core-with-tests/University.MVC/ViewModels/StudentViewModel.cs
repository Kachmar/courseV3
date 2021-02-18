using System;

namespace University.MVC.ViewModels
{
    public class StudentViewModel
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public DateTime BirthDate { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string GitHubLink { get; set; }

        public string Notes { get; set; }
        
    }
}