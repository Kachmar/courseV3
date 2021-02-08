using System.ComponentModel.DataAnnotations;

namespace University.MVC.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string Role { get; set; }
    }
}
