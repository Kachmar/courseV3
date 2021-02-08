using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace University.MVC.Authorization
{

    public class UkrainianAuthorizationHandler : AuthorizationHandler<UkrainianRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UkrainianRequirement requirement)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }

            if (context.User.HasClaim(claim => claim.ValueType == ClaimTypes.Country && claim.Value == "ukraine"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
