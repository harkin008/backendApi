using Microsoft.AspNet.Identity;
using SendGrid;
using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using test_webApiIdentity2.Controllers;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System.Runtime.Remoting.Contexts;

//using System.Runtime.Remoting.Contexts;


namespace test_webApiIdentity2.Services
{
    public class EmailService : IIdentityMessageService
    {
        public async  Task SendAsync(IdentityMessage message)
        {
            await  configSendGridasync(message);
        }

        // Use NuGet to install SendGrid (Basic C# client lib) 
        public async Task  configSendGridasync(IdentityMessage message)
        {
            try { 

            var userName = "harkin.tune";
            var msgFrm = "harkin.tune@gmail.com";
            var pswd = "h@rk1ntunde";



            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("smtp.gmail.com");
            client.Port = 587;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential cred = new System.Net.NetworkCredential(userName, pswd);
            client.EnableSsl = true;
            client.Credentials = cred;
            var mail = new System.Net.Mail.MailMessage(msgFrm, message.Destination);

            mail.Subject = message.Subject;
            mail.IsBodyHtml = true;
            mail.Body = message.Body + "<br/><br/><br/><br/><div>This message is sent from -------- app </div>";
           

            await client.SendMailAsync(mail);

            }
            catch (Exception ex)
            {
                throw new SmtpFailedRecipientException(ex.InnerException.Message);

                //throw new TaskCanceledException();
                
                //clearIncomplete clr = new clearIncomplete(message.Destination);
                    
               
                // delete user info from the database if error occur in sending mail
               //return ex.Message;
            }
        }
        private static string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            // Special "url-safe" base64 encode.
            return Convert.ToBase64String(inputBytes)
              .Replace('+', '-')
              .Replace('/', '_')
              .Replace("=", "");
        }
    }

    public class clearIncomplete : BaseApiController
    {
        public clearIncomplete(string mailTo)
        {
             var appUser = AppUserManager.FindByEmail(mailTo);
 
            if (appUser != null){

                AppUserManager.DeleteAsync(appUser);
            }
        }
   
    }
}
//------------------ google Api-------------------------------------//

//var msg = new AE.Net.Mail.MailMessage();
////{
////    Subject = "Account confirmation",
////    Body = "please click on the link below to activate your account"
////   // From = new MailAddress(msgFrm)
////};
//msg.Subject = "Account confirmation";
//msg.Body = "please click on the link below to activate your account";

//msg.To.Add(new MailAddress(message.Destination));
//msg.ReplyTo.Add(msg.From); // Bounces without this!!
//var msgStr = new StringWriter();
//msg.Save(msgStr);

//var gmail = new GmailService();
//var result = gmail.Users.Messages.Send(new Message
//{
//    Raw = Base64UrlEncode(msgStr.ToString())
//}, "me").Execute();
//Console.WriteLine("Message ID {0} sent.", result.Id);



///-----------------------------------sendgrid library-----------------------------------------////



//var transportWeb = new SendGrid.Web("SENDGRID_APIKEY");

//var msg = new SendGridMessage();
//msg.AddTo(message.Destination);
//msg.From = new MailAddress("harkin.tune@gmail.com", "Account Confirmation");
//msg.Subject = "Please confirm your Account";

//msg.Text = "";
//var trans = new SendGrid.Web("");
// trans.DeliverAsync(msg).Wait();




//      //// ---------------------------- .Net sendgrid smtp library----------------------------------////


//  var myMessage = new MailMessage();

//  myMessage.To.Add(message.Destination);

//  myMessage.From = new MailAddress("harkin.tune@gmail.com", "Confirm registration");

//  myMessage.Subject = message.Subject;
//  myMessage.Body = message.Body;
//  string text = "text body";
//  string html = @"<p>html body</p>";

//  myMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
//  myMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

////  myMessage.Html = message.Body;
//  var client = new System.Net.Mail.SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
//  client.Port = 587;
////  client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
//  client.UseDefaultCredentials = true;
//  // client.Credentials = new NetworkCredential(msgFrm, pswd);
//  client.EnableSsl = true;
//  //client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
//  client.ServicePoint.MaxIdleTime = 1;
// // client.UseDefaultCredentials = false;

//  client.Send(myMessage);


