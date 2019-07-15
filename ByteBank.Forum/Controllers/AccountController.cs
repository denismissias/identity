using ByteBank.Forum.Models;
using ByteBank.Forum.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ByteBank.Forum.Controllers
{
    public class AccountController : Controller
    {

        private UserManager<ApplicationUser> _userManager;

        public UserManager<ApplicationUser> UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    var owinContext = HttpContext.GetOwinContext();
                    _userManager = owinContext.GetUserManager<UserManager<ApplicationUser>>();

                }
                return _userManager;
            }
            set => _userManager = value;
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    FullName = model.FullName
                };

                var user = await UserManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    if (user.EmailConfirmed)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("WaitingConfirm");
                    }
                }

                var result = await UserManager.CreateAsync(newUser, model.Password);

                if (result.Succeeded)
                {
                    await SendConfirmEmail(newUser, ConfigurationManager.AppSettings["emailService:email_subject_confirm"], ConfigurationManager.AppSettings["emailService:email_body_confirm"]);
                    
                    return RedirectToAction("WaitingConfirm");
                }
                else
                {
                    AddErrors(result);
                }
            }

            return View(model);
        }

        public ActionResult WaitingConfirm()
        {
            return View();
        }

        public async Task<ActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return View("Error");
            }

            var result = await UserManager.ConfirmEmailAsync(userId, token);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Error");
            }
        }

        private async Task SendConfirmEmail(ApplicationUser user, string sbject, string body)
        {
            var token = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

            var callbackLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Url.Scheme);

            await UserManager.SendEmailAsync(user.Id, sbject, body.Replace("{callbackLink}", callbackLink));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }
    }
}