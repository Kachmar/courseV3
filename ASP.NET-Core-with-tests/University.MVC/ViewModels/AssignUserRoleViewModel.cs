using System.Collections.Generic;

namespace University.MVC.ViewModels
{
    public class AssignUserRoleViewModel
    {
        public string UserName { get; set; }

        public List<UserRoleViewModel> UserRoles { get; set; } = new List<UserRoleViewModel>();
    }

    public class UserRoleViewModel
    {
        public string RoleName { get; set; }
        public bool IsAssigned { get; set; }
    }
}