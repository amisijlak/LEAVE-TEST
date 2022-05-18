using LEAVE.DAL.Security;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LEAVE.DAL
{
    public interface IPermissionHelperService
    {
    }

    public class PermissionHelper : IPermissionHelperService
    {
        public PermissionHelper()
        {
        }

        private List<PermissionType> GetPermissionTypes()
        {
            var allControllers = GetAllControllers();

            var controllerAttrs = (from a in allControllers
                             from b in a.GetMethods()
                             where b.GetCustomAttributes().Any(r => r is IProtectActionAttribute)
                             from c in b.GetCustomAttributes().Where(r => r is IProtectActionAttribute)
                             select (IProtectActionAttribute)c);

            var razorPageAttrs = from a in GetAllRazorPages()
                                 where a.GetCustomAttributes().Any(r => r is IProtectActionAttribute)
                                 from b in a.GetCustomAttributes().Where(r => r is IProtectActionAttribute)
                                 select (IProtectActionAttribute)b;

            var protectedActionAttributes = controllerAttrs.Union(razorPageAttrs).Select(r => new
            {
                Module = r.Module,
                SubModule = r.SubModule,
                SystemAction = r.SystemAction
            }).Distinct()
                                             .Select(r => new PermissionType
                                             {
                                                 Module = r.Module,
                                                 SubModule = r.SubModule,
                                                 SystemAction = r.SystemAction
                                             }).ToList();

            return protectedActionAttributes;
        }

        private static IEnumerable<Type> GetAllControllers()
        {
            return from a in Assembly.GetExecutingAssembly().GetTypes().Where(r => r.FullName.StartsWith("LEAVE.DAL.Controllers")
                                     || (r.FullName.StartsWith("LEAVE.DAL.Areas") && r.FullName.Contains("Controllers")))
                   select a;
        }

        private static IEnumerable<Type> GetAllRazorPages()
        {
            return from a in Assembly.GetExecutingAssembly().GetTypes().Where(r => r.FullName.Contains(".Pages.") && 
                   typeof(PageModel).IsAssignableFrom(r))
                   select a;
        }
    }
}
