using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using University.MVC.ViewModels;

namespace University.MVC.Authorization
{
    public class SameStudentRequirementAuthorizationHandler : AuthorizationHandler<SameStudentRequirement, StudentViewModel>
    {
        public override Task HandleAsync(AuthorizationHandlerContext context)
        {
            return base.HandleAsync(context);
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            SameStudentRequirement requirement,
            StudentViewModel student)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }

            if (context.User.Identity?.Name == student.Email)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
