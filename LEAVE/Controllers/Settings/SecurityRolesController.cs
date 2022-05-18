using LEAVE.BLL.Data;
using LEAVE.BLL.Security;
using LEAVE.DAL;
using LEAVE.DAL.Models.Security;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LEAVE.Controllers.Settings
{
    public class SecurityRolesController : Controller
    {
        private readonly IDbRepository repository;
        private readonly ISecurityService securityService;

        public SecurityRolesController(IDbRepository repository, ISecurityService securityService)
        {
            this.repository = repository;
            this.securityService = securityService;
        }

        [ProtectAction(SecurityModule.Security, SecuritySubModule.SecurityRoles, SecuritySystemAction.ViewList)]
        public IActionResult Index(RoleContainerModel model)
        {
            model.LoadRoles(repository);
            return View(model);
        }

        [ProtectAction(SecurityModule.Security, SecuritySubModule.SecurityRoles, SecuritySystemAction.ViewList)]
        public IActionResult ReloadPermissions()
        {
            return RedirectToAction("Index", new { SuccessMessage = "Permission Reload Queued In Background." });
        }

        [ProtectAction(SecurityModule.Security, SecuritySubModule.SecurityRoles, SecuritySystemAction.CreateAndEdit)]
        public IActionResult Details(string Id = null)
        {
            return View(RoleContainerModel.GetRoleDetails(repository, Id));
        }

        [HttpPost]
        [ProtectAction(SecurityModule.Security, SecuritySubModule.SecurityRoles, SecuritySystemAction.CreateAndEdit)]
        public async Task<JsonResult> _Save(RoleContainerModel model)
        {
            return Json(new
            {
                success = await securityService.SaveRoleAsync(model),
                errorMessage = model.ErrorMessage
            });
        }

        [HttpPost]
        [ProtectAction(SecurityModule.Security, SecuritySubModule.SecurityRoles, SecuritySystemAction.Delete)]
        public JsonResult _Delete(string id)
        {
            string errorMessage = null;

            return Json(new
            {
                success = securityService.DeleteRole(id,out errorMessage),
                errorMessage = errorMessage
            });
        }
    }
}
