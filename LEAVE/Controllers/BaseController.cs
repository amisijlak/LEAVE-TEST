using System;
using LEAVE.BLL.Data;
using LEAVE.BLL.Security;
using Microsoft.AspNetCore.Mvc;

namespace LEAVE.DAL
{
    public abstract class BaseController : Controller
    {
        protected readonly ISessionService _sessionService;
        protected readonly IDbRepository _repository;

        public BaseController(IDbRepository repository, ISessionService sessionService)
        {
            _sessionService = sessionService;
            _repository = repository;
        }

        /// <summary>
        /// Have a fallback redirect for code that could fail. e.g. attempting to load the edit window for a record that doesn't exists.
        /// </summary>
        /// <param name="logicToExecute"></param>
        /// <param name="onErrorAction"></param>
        /// <returns></returns>
        protected IActionResult ExecuteSafeNavigation(Func<IActionResult> logicToExecute, string onErrorAction = "Index")
        {
            try
            {
                return logicToExecute();
            }
            catch (Exception e)
            {
                return RedirectToAction(onErrorAction, new { errorMessage = e.ExtractInnerExceptionMessage() });
            }
        }
    }
}
