using LEAVE.DAL.Security;
using LEAVE.DAL.Models.Security;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LEAVE.BLL.Data;
using LEAVE.DAL;
using LEAVE.BLL.Security;

namespace LEAVE.Controllers.Settings
{
    public class SystemUsersController : Controller
    {
        private readonly IDbRepository repository;
        private readonly ISecurityService securityService;
        private readonly ISessionService sessionService;

        public SystemUsersController(IDbRepository repository, ISecurityService securityService, ISessionService sessionService)
        {
            this.repository = repository;
            this.securityService = securityService;
            this.sessionService = sessionService;
        }

        [ProtectAction(SecurityModule.Security, SecuritySubModule.SystemUsers, SecuritySystemAction.ViewList)]
        public IActionResult Index(SystemUsersListModel model)
        {
            model.LoadUsers(repository);
            return View(model);
        }

        [ProtectAction(SecurityModule.Security, SecuritySubModule.SystemUsers, SecuritySystemAction.CreateAndEdit)]
        public IActionResult Create()
        {
            return View(new ApplicationUser());
        }

        [HttpPost]
        public PartialViewResult _GetUserRoleTemplate()
        {
            return PartialView("Partials/_UserRoleMapping");
        }

        [HttpPost]
        [ProtectAction(SecurityModule.Security, SecuritySubModule.SystemUsers, SecuritySystemAction.CreateAndEdit)]
        public async Task<JsonResult> _Create(ApplicationUser user, List<UserRoleModel> roles)
        {
            return Json(new
            {
                success = await securityService.CreateUserAsync(user, roles ?? new List<UserRoleModel>()
                , Url.Action("Login", "Account", new { UserName = user.UserName }, Request.Scheme)),
                errorMessage = user.ErrorMessage
            });
        }


        [ProtectAction(SecurityModule.Security, SecuritySubModule.SystemUsers, SecuritySystemAction.CreateAndEdit)]
        public IActionResult Edit(string Id)
        {
            try
            {
                var user = repository.Set<ApplicationUser>().Find(Id);
                if (user == null) throw new Exception("User Not Found!");

                if (user.IsSuperUser() && !sessionService.IsSuperAdmin())
                {
                    throw new Exception("User Not Accessible!");
                }

                return View(user);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", new { ErrorMessage = e.ExtractInnerExceptionMessage() });
            }
        }

        [HttpPost]
        [ProtectAction(SecurityModule.Security, SecuritySubModule.SystemUsers, SecuritySystemAction.CreateAndEdit)]
        public async Task<JsonResult> _Edit(ApplicationUser user, List<UserRoleModel> roles)
        {
            return Json(new
            {
                success = await securityService.UpdateUserAsync(user, roles ?? new List<UserRoleModel>()),
                errorMessage = user.ErrorMessage
            });
        }

        [HttpPost]
        [ProtectAction(SecurityModule.Security, SecuritySubModule.SystemUsers, SecuritySystemAction.Deactivate)]
        public JsonResult _Deactivate(LockoutModel model)
        {
            return Json(new
            {
                success = securityService.DeactivateUserAsync(model),
                errorMessage = model.ErrorMessage
            });
        }

        [HttpPost]
        [ProtectAction(SecurityModule.Security, SecuritySubModule.SystemUsers, SecuritySystemAction.Activate)]
        public JsonResult _Activate(LockoutModel model)
        {
            return Json(new
            {
                success = securityService.ActivateUserAsync(model),
                errorMessage = model.ErrorMessage
            });
        }
    }
}
