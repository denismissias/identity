using ByteBank.Forum.Models;
using ByteBank.Forum.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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

                var user = UserManager.FindByEmail(model.Email);

                if (user != null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var result = await UserManager.CreateAsync(newUser, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }

                
            }

            return View(model);
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