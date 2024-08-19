using Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Endpoint.CustomeAttributes
{
    public class AccessControlAttribute : ActionFilterAttribute
    {
        public String[] Roles { get; set; }



        public AccessControlAttribute(params string[] roles)
        {
            Roles = roles;
        }

        public int Sum(params int[] nums)
        {
            var res = 0;
            foreach (var item in nums)
            {
                res = item;
            }

            return res;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {


            if (context.HttpContext.User == null || context.HttpContext.User.Identity == null || !context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedObjectResult("برای انجام عملیات باید وارد شوید");
            }
            else
            {
                var role = context.HttpContext.User.FindFirstValue(AppClaims.Role);

                if (role == null)
                    context.Result = new UnauthorizedObjectResult("نشست شما به پایان رسیده");
                else if (!Roles.Contains(role))
                    context.Result = new StatusCodeResult(403);
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
