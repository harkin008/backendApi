using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace test_webApiIdentity2.Infrastructure
{
    public class ApplicationUserManager : UserManager<ApplicationUsers>
    {
        UserManager<ApplicationUsers> _userManager;
        public ApplicationUserManager(IUserStore<ApplicationUsers> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var appDbContext = context.Get<ApplicationDbContext>();
            var appUserManager = new ApplicationUserManager(new UserStore<ApplicationUsers>(appDbContext));

            appUserManager.UserValidator = new UserValidator<ApplicationUsers>(appUserManager)
            {
                AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true
            };
            appUserManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };


            appUserManager.EmailService = new test_webApiIdentity2.Services.EmailService();

           

	    var dataProtectionProvider = options.DataProtectionProvider;
	    if (dataProtectionProvider != null)
	    {
		    appUserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUsers>(dataProtectionProvider.Create("ASP.NET Identity"))
		{
			//Code for email confirmation and reset password life time
			TokenLifespan = TimeSpan.FromHours(6)
		};
	}


            return appUserManager;
        }

        //public async Task<IdentityUser> FindAsync(UserLoginInfo loginInfo)
        //{
        //    IdentityUser user = await _userManager.FindAsync(loginInfo);

        //    return user;
        //}

        //public async Task<IdentityResult> CreateAsync(ApplicationUsers  user)
        //{
        //    var result = await _userManager.CreateAsync(user);

        //    return result;
        //}

        //public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        //{
        //    var result = await _userManager.AddLoginAsync(userId, login);

        //    return result;
        //}
    }
}
