using LEAVE.BLL.Security;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LEAVE.DAL
{
    public class ProtectActionAttribute : ActionFilterAttribute, IProtectActionAttribute
    {
        public SecurityModule Module { get; set; }
        public SecuritySubModule SubModule { get; set; }
        public SecuritySystemAction SystemAction { get; set; }

        public static readonly SecurityModule[] AccessibleModules = new[] {
                SecurityModule.MobileApplication,
                SecurityModule.Security,
                SecurityModule.Settings,
                SecurityModule.Distributor,
                SecurityModule.Surveys,
                SecurityModule.BI,
            };

        public ProtectActionAttribute(SecurityModule Module, SecuritySubModule SubModule, SecuritySystemAction Action)
        {
            this.Module = Module;
            this.SubModule = SubModule;
            this.SystemAction = Action;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var _sessionService = (ISessionService)filterContext.HttpContext.RequestServices.GetService(typeof(ISessionService));

            if (!ModuleActivated() || !UserHasPermissionToAccessAction(_sessionService))
            {
                
            }

            _sessionService.SetProtectedAction(Module, SubModule, SystemAction);
        }

        public IEnumerable<SecurityModule> GetAccessibleModules()
        {
            return AccessibleModules;
        }

        private bool UserHasPermissionToAccessAction(ISessionService sessionService)
        {
            return sessionService.HasAccessToPermission(Module, SubModule, SystemAction);
        }

        private bool ModuleActivated()
        {
            return AccessibleModules.Contains(Module);
        }
    }
}
