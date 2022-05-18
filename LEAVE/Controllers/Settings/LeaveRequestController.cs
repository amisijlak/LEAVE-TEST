using LEAVE.DAL.Settings;
using LEAVE.DAL.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using LEAVE.BLL.Leave;
using LEAVE.DAL;

namespace LEAVE.Controllers.Settings
{
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestService leaveService;
        public LeaveRequestController(ILeaveRequestService leaveService)
        {
            this.leaveService = leaveService;
        }

        public IActionResult Index(SettingsListModel model)
        {
            model.LoadELeaveRequests(leaveService);
            return View(model);
        }

        [ProtectAction(SecurityModule.Settings, SecuritySubModule.Leave_Request, SecuritySystemAction.CreateAndEdit)]
        public IActionResult Details(long Id = 0)
        {
            return View(leaveService.GetLeaveRequests().Where(r => r.Id == Id).SingleOrDefault() ?? new LeaveRequest());
        }

        [HttpPost]
        [ProtectAction(SecurityModule.Settings, SecuritySubModule.Leave_Request, SecuritySystemAction.CreateAndEdit)]
        public JsonResult _Save(LeaveRequest model)
        {
            var result = leaveService.SaveLeaveRequest(model);
            return Json(new
            {
                success = result.Item1,
                errorMessage = result.Item2
            });
        }
    }
}
