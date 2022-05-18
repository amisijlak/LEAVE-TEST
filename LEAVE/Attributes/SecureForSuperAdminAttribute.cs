using LEAVE.BLL.Security;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace LEAVE.DAL
{
    public class SecureForSuperAdminAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var _sessionService = ((ISessionService)context.HttpContext.RequestServices.GetService(typeof(ISessionService)));

            if (!DALExtensionMethods.IsSuperUser(_sessionService.GetUserName()))
            {
               
            }
        }
    }
}
