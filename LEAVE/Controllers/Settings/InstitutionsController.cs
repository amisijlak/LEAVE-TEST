using LEAVE.BLL.Data;
using LEAVE.BLL.Settings;
using LEAVE.DAL.Entities;
using LEAVE.DAL.Models.Settings;
using Microsoft.AspNetCore.Mvc;

namespace LEAVE.DAL.Controllers.Settings
{
    public class InstitutionsController : Controller
    {
        private readonly ISettingsService settingsService;
        private readonly IDbRepository repository;

        public InstitutionsController(IDbRepository repository, ISettingsService settingsService)
        {
            this.repository = repository;
            this.settingsService = settingsService;
        }

        [ProtectAction(SecurityModule.Settings, SecuritySubModule.Institutions, SecuritySystemAction.ViewList)]
        public IActionResult Index(SettingsListModel model)
        {
            model.LoadData(repository);
            return View(model);
        }

        [ProtectAction(SecurityModule.Settings, SecuritySubModule.Institutions, SecuritySystemAction.CreateAndEdit)]
        public IActionResult Details(long Id = 0)
        {
            return View(repository.Set<Institution>().Find(Id) ?? new Institution());
        }

        [HttpPost]
        [ProtectAction(SecurityModule.Settings, SecuritySubModule.Institutions, SecuritySystemAction.CreateAndEdit)]
        public JsonResult _Save(Institution model)
        {
            string errorMessage = null;

            return Json(new
            {
                success = settingsService.SaveCodedModel(model, out errorMessage),
                errorMessage
            });
        }

        [HttpPost]
        [ProtectAction(SecurityModule.Settings, SecuritySubModule.Institutions, SecuritySystemAction.Delete)]
        public JsonResult _Delete(long Id)
        {
            string errorMessage = null;

            return Json(new
            {
                success = settingsService.DeleteModel<Institution>(Id, out errorMessage),
                errorMessage
            });
        }
    }
}
