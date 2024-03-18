using Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
            var role = context.HttpContext.User.Claims.First(b => b.Type == AppClaims.Role);

            if (role == null)
                context.Result = new BadRequestObjectResult("به این صفحه دسترسی ندارید");
            else if (role.Value != Role)
                context.Result = new BadRequestObjectResult("به این صفحه دسترسی ندارید");

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
