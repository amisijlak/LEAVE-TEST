using LEAVE.BLL.Data;
using LEAVE.BLL.Settings;
using LEAVE.DAL.Lookups;
using LEAVE.DAL.Models.Settings;
using Microsoft.AspNetCore.Mvc;

namespace LEAVE.DAL.Controllers.Settings
{
    public class GenericLookupController : Controller
    {
        private readonly ISettingsService settingsService;
        private readonly IDbRepository repository;

        public GenericLookupController(IDbRepository repository, ISettingsService settingsService)
        {
            this.repository = repository;
            this.settingsService = settingsService;
        }

        [ProtectAction(SecurityModule.Settings, SecuritySubModule.GenericLookups, SecuritySystemAction.ViewList)]
        public IActionResult Index(SettingsListModel model)
        {
            model.LoadGenericLookups(repository);
            return View(model);
        }

        [ProtectAction(SecurityModule.Settings, SecuritySubModule.GenericLookups, SecuritySystemAction.CreateAndEdit)]
        public IActionResult Details(long Id = 0)
        {
            return View(repository.Set<GenericLookup>().Find(Id) ?? new GenericLookup());
        }

        [HttpPost]
        [ProtectAction(SecurityModule.Settings, SecuritySubModule.GenericLookups, SecuritySystemAction.CreateAndEdit)]
        public JsonResult _Save(GenericLookup model)
        {
            string errorMessage = null;

            return Json(new
            {
                success = settingsService.SaveCodedModel(model, out errorMessage),
                errorMessage
            });
        }

        [HttpPost]
        [ProtectAction(SecurityModule.Settings, SecuritySubModule.GenericLookups, SecuritySystemAction.Delete)]
        public JsonResult _Delete(long Id)
        {
            string errorMessage = null;

            return Json(new
            {
                success = settingsService.DeleteModel<GenericLookup>(Id, out errorMessage),
                errorMessage
            });
        }
    }
}
