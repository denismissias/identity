using ByteBank.Forum.App_Start.Identity;
using ByteBank.Forum.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Owin;
using System.Data.Entity;

[assembly: OwinStartup(typeof(ByteBank.Forum.Startup))]

namespace ByteBank.Forum
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            builder.CreatePerOwinContext<DbContext>(() => new IdentityDbContext<ApplicationUser>("DefaultConnection"));

            builder.CreatePerOwinContext<IUserStore<ApplicationUser>>((options, owinContext) =>
            {
                var dbContext = owinContext.Get<DbContext>();

                return new UserStore<ApplicationUser>(dbContext);
            });

            builder.CreatePerOwinContext<UserManager<ApplicationUser>>((options, owinContext) =>
            {
                var userStore = owinContext.Get<IUserStore<ApplicationUser>>();
                var userManager = new UserManager<ApplicationUser>(userStore);

                var userValidator = new UserValidator<ApplicationUser>(userManager)
                {
                    RequireUniqueEmail = true
                };

                userManager.UserValidator = userValidator;

                userManager.PasswordValidator = new PasswordValidator()
                {
                    RequireDigit = true,
                    RequiredLength = 6,
                    RequireLowercase = true,
                    RequireNonLetterOrDigit = true,
                    RequireUppercase = true
                };

                userManager.EmailService = new EmailService();

                userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(options.DataProtectionProvider.Create("ByteBank.Forum"));

                return userManager;
            });
        }
    }
}