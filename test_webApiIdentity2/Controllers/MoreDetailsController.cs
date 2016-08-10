using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using test_webApiIdentity2.Models;
using test_webApiIdentity2.Infrastructure;
using System.Web.Http.Cors;
using System.IO;
using System.Web;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;


namespace test_webApiIdentity2.Controllers
{
    [RoutePrefix("api/more")]
    [EnableCors(origins: "http://localhost:35684", headers: "accept, content-type:application/x-www-form-urlencoded", methods: "*")]
    
    public class MoreDetailsController : ApiController
    {
        
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET api/MoreDetails
        [Authorize]
        [Route("all")]
        public IQueryable<user_MoreDetails> Getuser_MoreDetails()
        {
            return db.user_MoreDetails;
        }

         

        [Route("single/{id}")]//"user/{id:guid}", Name = "GetUserById")
        // GET api/MoreDetails/5
        [ResponseType(typeof(user_MoreDetails))]
        public async Task<IHttpActionResult> Getuser_MoreDetails(string id)
        {
            user_MoreDetails user_moredetails = await db.user_MoreDetails.FindAsync(id);
            if (user_moredetails == null)
            {
                return NotFound();
            }

            return Ok(user_moredetails);
        }

        // PUT api/MoreDetails/5
        public async Task<IHttpActionResult> Putuser_MoreDetails(string id, user_MoreDetails user_moredetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user_moredetails.userID)
            {
                return BadRequest();
            }

            db.Entry(user_moredetails).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!user_MoreDetailsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/MoreDetails
        [Route("addmore")]
        [ResponseType(typeof(user_MoreDetails))]
        public async Task<IHttpActionResult> Postuser_MoreDetails(user_MoreDetails user_moredetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            //user_moredetails.userID = "";
            db.user_MoreDetails.Add(user_moredetails);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (user_MoreDetailsExists(user_moredetails.userID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = user_moredetails.userID }, user_moredetails);
        }

        // DELETE api/MoreDetails/5
        [ResponseType(typeof(user_MoreDetails))]
        public async Task<IHttpActionResult> Deleteuser_MoreDetails(string id)
        {
            user_MoreDetails user_moredetails = await db.user_MoreDetails.FindAsync(id);
            if (user_moredetails == null)
            {
                return NotFound();
            }

            db.user_MoreDetails.Remove(user_moredetails);
            await db.SaveChangesAsync();

            return Ok(user_moredetails);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool user_MoreDetailsExists(string id)
        {
            return db.user_MoreDetails.Count(e => e.userID == id) > 0;
        }
        
        [HttpPost]
        [Route("uploadImage")]
        public string UploadFiles()
        {

            //if (Request.Content.IsMimeMultipartContent())
            //{

                int iUploadedCnt = 0;

                // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/uploads/");


               // var requestfil = HttpContext.Current.Request.Form.AllKeys["id"];
                //  var req = Request.Files;

                HttpPostedFile hfc = System.Web.HttpContext.Current.Request.Files["file"];
                var req = HttpContext.Current.Request;

                if (hfc == null)
                {
                    return "posted file error";
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                    
                }
                else if (hfc != null)
                {
                    try
                    {
                        string fileID = HttpContext.Current.Request.Params.GetValues(0).GetValue(0).ToString();
                        string filename = hfc.FileName;
                        if (!Directory.Exists("sPath  + fileID/"))
                        {
                            Directory.CreateDirectory(sPath + fileID);
                        }
                        hfc.SaveAs(sPath  + fileID + "/" + filename);
                        return "Successfully uploaded: " + filename;
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }

                // CHECK THE FILE COUNT.
                //for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                //{
                //    System.Web.HttpPostedFile hpf = hfc[iCnt];

                //    if (hpf.ContentLength > 0)
                //    {
                //        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                //        if (!File.Exists(sPath + Path.GetFileName(hpf.FileName)))
                //        {
                //            // SAVE THE FILES IN THE FOLDER.
                //            hpf.SaveAs(sPath + Path.GetFileName(hpf.FileName));
                //            iUploadedCnt = iUploadedCnt + 1;
                //        }
                //    }
                //  }

                // RETURN A MESSAGE (OPTIONAL).
                if (iUploadedCnt > 0)
                {
                    return iUploadedCnt + " Files Uploaded Successfully";
                }
                else
                {
                    return "Upload Failed";
                }
           //}
            return "Upload not successful";
        }
    }


    public class uploadedParam
    {
        public string id { get; set; }
        public HttpPostedFileBase file { get; set; }
    }


  
}