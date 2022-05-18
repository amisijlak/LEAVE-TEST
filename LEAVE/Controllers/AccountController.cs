using LEAVE.DAL.Security;
using LEAVE.DAL.Models;
using LEAVE.DAL.Models.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LEAVE.BLL.Security;
using LEAVE.BLL.Data;
using LEAVE.BLL.Helpers;

namespace LEAVE.DAL.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISessionService _sessionService;
        private readonly ISecurityService _securityService;
        private readonly IDbRepository _repository;
        private readonly IEmailSender _emailSender;

        public AccountController(SignInManager<ApplicationUser> SignInManager, UserManager<ApplicationUser> UserManager, ISessionService _sessionService
            , ISecurityService _securityService, IDbRepository _repository, IEmailSender _emailSender)
        {
            this._signInManager = SignInManager;
            this._userManager = UserManager;
            this._sessionService = _sessionService;
            this._securityService = _securityService;
            this._repository = _repository;
            this._emailSender = _emailSender;
        }

        [AllowAnonymous]
        public IActionResult Login(string UserName, string ReturnUrl)
        {
            if (Request.Cookies.Keys.Any())
            {
                try
                {
                    foreach (var cookie in Request.Cookies.Keys)
                    {
                        if (!cookie.ToLower().Contains("antiforgery")) Response.Cookies.Delete(cookie);
                    }
                }
                catch (Exception e) { }
            }

            return View(new LoginViewModel { UserName = UserName });
        }

        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dbUser = await _userManager.GetUserByUsernameOrEmailAsync(model.UserName);

                var result = await _signInManager.PasswordSignInAsync(dbUser?.UserName ?? model.UserName, model.Password, false, true);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
            }
            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            var userId = _sessionService.GetUserId();

            _sessionService.DestroyCookies();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Lockout()
        {
            if (User.Identity.IsAuthenticated) await _signInManager.SignOutAsync();
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordModel());
        }

        [HttpPost]
        public async Task<JsonResult> _changePassword(ChangePasswordModel model)
        {
            var success = await _securityService.ChangePasswordAsync(model);

            return Json(new
            {
                success,
                errorMessage = model.ErrorMessage
            });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Reload()
        {
            await _securityService.UnlockSuperAdmin();

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null || string.IsNullOrEmpty(user.Email))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                //Send an email with this link
                string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, Request.Scheme);
                //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

                string ErrorMessage = null;

                if (!_emailSender.SendEmail(new[] { user.Email }, $"LEAVE Password Reset Link"
                    , $"Dear {user.FirstName},<br/><br/>Please reset your password by clicking <a href=\"" + callbackUrl + "\" target='_blank'>here</a>.<br/>"
                    + "<br/>If you wish not to reset your password, simply ignore this email and login with your current password.",
                    out ErrorMessage))
                {
                }

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
