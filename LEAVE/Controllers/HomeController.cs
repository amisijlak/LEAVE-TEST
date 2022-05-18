using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LEAVE.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using LEAVE.BLL.Security;
using LEAVE.BLL.Data;

namespace LEAVE.DAL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDbRepository _repository;
        private readonly ISessionService _sessionService;

        public HomeController(IDbRepository repository, ILogger<HomeController> logger, ISessionService sessionService)
        {
            _logger = logger;
            _repository = repository;
            _sessionService = sessionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult UnAuthorized()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true), AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
