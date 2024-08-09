using Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Endpoint.CustomeAttributes
{
    public class AccessControlAttribute : ActionFilterAttribute
    {
        public dynamic Role { get; set; }

        public AccessControlAttribute(string role)
        {
            Role = role;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var role = context.HttpContext.User.FindFirstValue(AppClaims.Role);

            if (role == null)
                context.Result = new UnauthorizedObjectResult("نشست شما به پایان رسیده");
            else if (role != Role)
                context.Result = new StatusCodeResult(403);

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
