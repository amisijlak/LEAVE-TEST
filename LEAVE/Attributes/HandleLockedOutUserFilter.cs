using LEAVE.BLL.Security;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace LEAVE.DAL
{
    public class HandleLockedOutUserFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var descriptor = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor;

            if (filterContext.HttpContext.User.Identity.IsAuthenticated && descriptor.ActionName.ToLower() != "lockout")
            {
                var _sessionService = (ISessionService)filterContext.HttpContext.RequestServices.GetService(typeof(ISessionService));

                if (_sessionService.GetUser()?.LockoutEnd > DateTime.Now)
                {
                    
                }
            }
        }
    }
}
