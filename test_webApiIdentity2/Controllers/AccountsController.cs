using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using test_webApiIdentity2.Infrastructure;
using test_webApiIdentity2.Models;
using System.Web.Http.Cors;
using System.IO;
using test_webApiIdentity2.Filters;


namespace test_webApiIdentity2.Controllers
{
   //[test_webApiIdentity2.Filters.ForceHttps]
    [RoutePrefix("api/accounts")]
    [EnableCors(origins: "http://localhost:35684", headers: "Accept, content-type", methods: "*")]
      
    public class AccountsController : BaseApiController
    {   
        //[test_webApiIdentity2.Filters.ForceHttps ]
       
       [AllowAnonymous]
      //  [Authorize]
       [Route("users")]
        public IHttpActionResult GetUsers()
        {
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }
       
        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }
        [Authorize(Roles = "Admin")]
        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await this.AppUserManager.FindByNameAsync(username);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }
        //[Authorize(Roles = "Admin")]
         [AllowAnonymous]
        [Route("create")]
        public async Task<IHttpActionResult> CreateUser(CreateUserBindingModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUsers()
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                FirstName = createUserModel.FirstName,
                LastName = createUserModel.LastName,
                Level = 3,
                JoinDate = DateTime.Now.Date,
            };

            IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);

            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }
             string code = await this.AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { userId = user.Id, code = code }));

              var ret =  AppUserManager.SendEmailAsync(user.Id,"Confirm your account", "<h2> Hello " + user.Id +" </h2> <br/><br/> Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>").Exception;

              if (ret != null )
              {
                  var appUser = await this.AppUserManager.FindByIdAsync(user.Id);

                  if (appUser != null)
                  {
                      IdentityResult result = await this.AppUserManager.DeleteAsync(appUser);

                      if (result.Succeeded)
                      {
                          ModelState.AddModelError("Messaging Error", ret.InnerException);
                          return BadRequest(ModelState);
                      }

                      return Ok();

                  }

                  return Ok();
                 
              
                  
              }
              else
              {

                  Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

                  return Created(locationHeader, TheModelFactory.Create(user));

              }
                    
        }
         [HttpGet]
         [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
         public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
         {
             if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
             {
                 ModelState.AddModelError("", "User Id and Code are required");
                 return BadRequest(ModelState);
             }

             IdentityResult result = await this.AppUserManager.ConfirmEmailAsync(userId, code);

             if (result.Succeeded)
             {
                 return Ok();
             }
             else
             {
                 return GetErrorResult(result);
             }
         }

         [Route("ChangePassword")]
         public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
         {
             if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }

             IdentityResult result = await this.AppUserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

             if (!result.Succeeded)
             {
                 return GetErrorResult(result);
             }

             return Ok();
         }

       [Route("user/{id:guid}")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
 
            //Only SuperAdmin or Admin can delete users (Later when implement roles)
 
            var appUser = await this.AppUserManager.FindByIdAsync(id);
 
            if (appUser != null)
            {
                IdentityResult result = await this.AppUserManager.DeleteAsync(appUser);
 
                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
 
                return Ok();
 
            }
 
            return NotFound();
          
        }


    }
}

