using AspNetIdentity.WebApi.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using test_webApiIdentity2.Infrastructure;
using test_webApiIdentity2.Providers;
using System.Web.Http.Cors;
using test_webApiIdentity2.Filters;
using System.Net.Http.Headers;

namespace test_webApiIdentity2
{
    public class Startup
    {
       [EnableCors(origins: "*", headers: "accept:application/json, content-type:application/json;charset=utf-8", methods: "*")]
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();

            httpConfig.Formatters.Add(new imageUploadFormatter());

            ConfigureWebApi(httpConfig);
           // enforce https
            //httpConfig.Filters.Add(new ForceHttpsAttribute());
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            
          // app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions());
           httpConfig.EnableCors();
         //  httpConfig.MapHttpAttributeRoutes();

            ConfigureOAuthTokenGeneration(app);
            ConfigureOAuthTokenConsumption(app);

          
            app.UseWebApi(httpConfig);
          //  GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();


        }

        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Plugin the OAuth bearer JSON Web Token tokens generation and Consumption will be here

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new CustomOAuthProvider(),
                AccessTokenFormat = new CustomJwtFormat("http://localhost:19330")
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);

        }

        private void ConfigureWebApi(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();

            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.Formatters.Add( new imageUploadFormatter());


         //   var medialFormatter = config.Formatters.OfType<MediaTypeFormatter>().First();
           // medialFormatter.SetDefaultContentHeaders(MediaTypeFormatter.GetDefaultValueForType, "", "");
        }
        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {

            var issuer = "http://localhost:19330";
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                    }
                });
        }
    }
}