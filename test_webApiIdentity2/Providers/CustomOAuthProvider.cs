using test_webApiIdentity2.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace test_webApiIdentity2.Providers
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        UserManager<ApplicationUsers> _userManager;

        // [EnableCors(origins: "http://localhost:35684", headers: "accept:application/json, content-type:application/json;charset=utf-8, Access-Control-Allow-Origin:http://localhost:35684", methods: "*")]
      
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            var allowedOrigin = "http://localhost:35684";

            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Credentials", new []{"true"});
           //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            ApplicationUsers user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            if (!user.EmailConfirmed)
            {
                context.SetError("invalid_grant", "User did not confirm email.");
                return;
            }

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");

            //oAuthIdentity.AddClaims(ExtendedClaimsProvider.GetClaims(user));
            //oAuthIdentity.AddClaims(RolesFromClaims.CreateRolesBasedOnClaims(oAuthIdentity));
           
            var ticket = new AuthenticationTicket(oAuthIdentity, null);
            
            context.Validated(ticket);
           
        }

        public async Task<IdentityUser> FindAsync(UserLoginInfo loginInfo)
        {
            IdentityUser user = await _userManager.FindAsync(loginInfo);

            return user;
        }

        // changed in case
        public async Task<IdentityResult> CreateAsync(ApplicationUsers  user)
        {
            var result = await _userManager.CreateAsync(user);
            
            return result;
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            var result = await _userManager.AddLoginAsync(userId, login);

            return result;
        }
    }
}