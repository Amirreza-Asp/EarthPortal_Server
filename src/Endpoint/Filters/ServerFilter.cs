using Microsoft.AspNetCore.Mvc.Filters;

namespace Endpoint.Filters
{
    public class RemoveServerInfoFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext != null)
            {
                filterContext.HttpContext.Response.Headers.Remove("Server");
            }
        }
    }
}
