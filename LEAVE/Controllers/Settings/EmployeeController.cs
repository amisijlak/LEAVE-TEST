using LEAVE.DAL.Settings;
using LEAVE.DAL.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using LEAVE.BLL.Employees;

namespace LEAVE.DAL.Controllers.Settings
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public IActionResult Index(SettingsListModel model)
        {
            model.LoadEmployees(employeeService);
            return View(model);
        }

        [ProtectAction(SecurityModule.Settings, SecuritySubModule.Employee, SecuritySystemAction.CreateAndEdit)]
        public IActionResult Details(long Id = 0)
        {
            return View(employeeService.GetEmployees().Where(r=>r.Id == Id).SingleOrDefault() ?? new Employee());
        }

        [HttpPost]
        [ProtectAction(SecurityModule.Settings, SecuritySubModule.Employee, SecuritySystemAction.CreateAndEdit)]
        public JsonResult _Save(Employee model)
        {
            var result = employeeService.SaveEmployee(model);
            return Json(new
            {
                success = result.Item1,
                errorMessage = result.Item2
            });
        }

    }
}
