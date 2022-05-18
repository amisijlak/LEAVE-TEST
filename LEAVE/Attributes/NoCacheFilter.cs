using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LEAVE.DAL
{
    /// <summary>
    /// Disable Caching
    /// </summary>
    public class NoCacheFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext?.HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store");
            filterContext?.HttpContext.Response.Headers.Add("Expires", "-1");
        }
    }
}
