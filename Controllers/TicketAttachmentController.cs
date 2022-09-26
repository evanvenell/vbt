using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using BugTrackerApplication.DAL;
using BugTrackerApplication.Models;
using System.Data.Entity.Infrastructure;
using BugTrackerApplication.ViewModels;
using BugTrackerApplication.Filters;
using BugTrackerApplication.CustomFilters;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.IO;

namespace BugTrackerApplication.Controllers
{
    public class TicketAttachmentController : Controller
    {
        private ApplicationContext db = new ApplicationContext();

        // GET: TicketAttachment
        //public ActionResult Index()
        //{
        //    return View();
        //}
       
        //
        //  GET: TTicketAttachment
        public async Task<ActionResult> Index()
        {
            var attachments = db.TicketAttachments.Include(c => c.Ticket);
            var attachCount = attachments.Count();
            ViewBag.NumberOfAttachments = attachCount.ToString();
            return View(await attachments.ToListAsync());
        }

        // GET: Comment (ORIGINAL)
        //[AuthLog(Roles = "Admin")] //NOTE: The index screen never really gets used so other roles don't need access. This avoids confusion since the coments should be viewed from within their corresponding parent tickets. -EV 
        //public async Task<ActionResult> Index()
        //{
        //    var comments = db.Comments.Include(c => c.Ticket);
        //    return View(await comments.ToListAsync());
        //}

        //
        //  GET: TicketAttachment/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachments attachment = await db.TicketAttachments.FindAsync(id);
            if (attachment == null)
            {
                return HttpNotFound();
            }
            return View(attachment);
        }

        // GET: Comment/Details/5 (ORIGINAL)
        //[AuthLog(Roles = "Admin,Developer,Project Manager,Basic,Demo_Admin,Demo_Developer,Demo_ProjectManager,Demo_Submitter")]
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Comment comment = await db.Comments.FindAsync(id);
        //    if (comment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(comment);
        //}
        public FileContentResult Photo(string userId)
        {
            var db = HttpContext.GetOwinContext().Get<ApplicationContext>();

            var user = db.Users.Where(u => u.Id == userId).FirstOrDefault();

            return new FileContentResult(user.ProfilePicture, "image/jpeg");
        }

        //
        //  GET: TicketAttchment/Create
        [HttpGet]
        public ActionResult Create()
        {
            //Link attachment to ticket //
            ViewBag.TicketID = TempData["AddAttachment"];
            ViewBag.AuthorID = TempData["AuthorID"];
            return View();
        }

        //
        //  POST: TicketAttchment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[AuthLog(Roles = "Admin,Developer,Project Manager,Basic")]
        //public async Task<ActionResult> Create([Bind(Include = "ID,TicketID,CommentTitle,Commenter,Message,Created")] Comment comment, int? ticketID)
        public async Task<ActionResult> Create([Bind(Include = "ID,TicketID,Title,FileName,Author,Uploaded")] TicketAttachments attachment, int? ticketID, HttpPostedFileBase files)
        { //NOTE: The Demo_Roles will not have permission to actually SaveChanges() only to see the interface.


            if (ModelState.IsValid)
            {
                // Auto Assign the Commenter prop based on the matching authenticated profile to the corresponding dev or sub profile. 
                var userStore = new UserStore<ApplicationUser>(db);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var userId = User.Identity.GetUserId();
                ApplicationUser user = db.Users.Where(u => u.Id.Equals(userId)).FirstOrDefault();
                // Upload File Attachment //
                

                //------------------------//
                // Developer assign
                if (user.UserType == UserType.Employee)
                {
                    
                    var queryDeveloperUserID = db.Developers.Where(u => u.UserId.Equals(
                        user.Id, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
                    if (queryDeveloperUserID != null)
                    {
                        var DeveloperName = queryDeveloperUserID.FullName;
                        //comment.Commenter = DeveloperName;
                        attachment.Author = DeveloperName;
                        
                        //Title
                        
                        // ./Title

                    }
                }


                // Submitter assign
                if (user.UserType == UserType.Customer)
                {
                    var querySubmitterUserID = db.Submitters.Where(u => u.UserId.Equals(
                        user.Id, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
                    if (querySubmitterUserID != null)
                    {
                        var SubmitterName = querySubmitterUserID.FullName;
                        //comment.Commenter = SubmitterName;
                        attachment.Author = SubmitterName;
                    }
                }
                if (files != null && files.ContentLength > 0)
                {
                    // Extract only the filename
                    var fileName = Path.GetFileName(files.FileName);
                    // Store the file inside ~/App_Data/AttachmentFiles folder
                    var path = Path.Combine(Server.MapPath(@"~/App_Data/AttachmentFiles"), fileName);
                    files.SaveAs(path);
                    attachment.FileName = fileName;
                }


                // Auto assign the CreatedDate prop//
                attachment.Uploaded = System.DateTime.Now;
                db.TicketAttachments.Add(attachment);
                await db.SaveChangesAsync();

                

                //-- Update the Updated Prop for comment ticket --//
                var ticketTimestampToUpdate = db.Tickets
                    .Where(i => i.TicketID == attachment.TicketID).SingleOrDefault();
                ticketTimestampToUpdate.Updated = System.DateTime.Now;
                await db.SaveChangesAsync();
                //return RedirectToAction("Index", "Ticket");
                //return RedirectToAction("Details", "Ticket", ViewBag.TicketID);
                return RedirectToAction("Details", "Ticket", new { id = attachment.TicketID }); //NOTE: This is meant to redirect back to the original detail screen for the ticket
                                                                                             //clicked to get to this comment creation screen BUT having issues with the id piece of the URL, will return later. -EV 03/22/2021...
            }

            //ViewBag.TicketID = new SelectList(db.Tickets, "TicketID", "TicketTitle", comment.TicketID);
            //ViewBag.AddComment = TempData["AddComment"];
            ViewBag.TicketID = TempData["AddAttachment"];

            //TempData["CommentAdded"] = ViewBag.TicketID;
            return View(attachment);
        }


        // POST: Comment/Create (ORIGINAL)
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AuthLog(Roles = "Admin,Developer,Project Manager,Basic")]
        ////public async Task<ActionResult> Create([Bind(Include = "ID,TicketID,CommentTitle,Commenter,Message,Created")] Comment comment)
        //public async Task<ActionResult> Create([Bind(Include = "ID,TicketID,CommentTitle,Commenter,Message,Created")] Comment comment, int? ticketID)
        //{ //NOTE: The Demo_Roles will not have permission to actually SaveChanges() only to see the interface.


        //    if (ModelState.IsValid)
        //    {
        //        // Auto Assign the Commenter prop based on the matching authenticated profile to the corresponding dev or sub profile. 
        //        var userStore = new UserStore<ApplicationUser>(db);
        //        var userManager = new UserManager<ApplicationUser>(userStore);

        //        var userId = User.Identity.GetUserId();
        //        ApplicationUser user = db.Users.Where(u => u.Id.Equals(userId)).FirstOrDefault();

        //        // Developer assign
        //        if (user.UserType == UserType.Employee)
        //        {
        //            var queryDeveloperUserID = db.Developers.Where(u => u.UserId.Equals(
        //                user.Id, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
        //            if (queryDeveloperUserID != null)
        //            {
        //                var DeveloperName = queryDeveloperUserID.FullName;
        //                comment.Commenter = DeveloperName;
        //            }
        //        }


        //        // Submitter assign
        //        if (user.UserType == UserType.Customer)
        //        {
        //            var querySubmitterUserID = db.Submitters.Where(u => u.UserId.Equals(
        //                user.Id, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
        //            if (querySubmitterUserID != null)
        //            {
        //                var SubmitterName = querySubmitterUserID.FullName;
        //                comment.Commenter = SubmitterName;
        //            }
        //        }

        //        // Auto assign the CreatedDate prop//
        //        comment.Created = System.DateTime.Now;
        //        db.Comments.Add(comment);
        //        await db.SaveChangesAsync();
        //        //-- Update the Updated Prop for comment ticket --//
        //        var ticketTimestampToUpdate = db.Tickets
        //            .Where(i => i.TicketID == comment.TicketID).SingleOrDefault();
        //        ticketTimestampToUpdate.Updated = System.DateTime.Now;
        //        await db.SaveChangesAsync();
        //        //return RedirectToAction("Index", "Ticket");
        //        //return RedirectToAction("Details", "Ticket", ViewBag.TicketID);
        //        return RedirectToAction("Details", "Ticket", new { id = comment.TicketID }); //NOTE: This is meant to redirect back to the original detail screen for the ticket
        //                                                                                     //clicked to get to this comment creation screen BUT having issues with the id piece of the URL, will return later. -EV 03/22/2021...
        //    }

        //    //ViewBag.TicketID = new SelectList(db.Tickets, "TicketID", "TicketTitle", comment.TicketID);
        //    //ViewBag.AddComment = TempData["AddComment"];
        //    ViewBag.TicketID = TempData["AddComment"];

        //    //TempData["CommentAdded"] = ViewBag.TicketID;
        //    return View(comment);
        //}



        // GET: Comment/Edit/5
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic,Demo_Admin,Demo_Developer,Demo_ProjectManager,Demo_Submitter")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = await db.Comments.FindAsync(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketID = new SelectList(db.Tickets, "TicketID", "TicketTitle", comment.TicketID);
            return View(comment);
        }

        // POST: Comment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic")]
        public async Task<ActionResult> Edit([Bind(Include = "ID,TicketID,CommentTitle,Commenter,Message,Created")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.TicketID = new SelectList(db.Tickets, "TicketID", "TicketTitle", comment.TicketID);
            return View(comment);
        }

        // GET: Comment/Delete/5
        [AuthLog(Roles = "Admin,Demo_Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = await db.Comments.FindAsync(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Comment comment = await db.Comments.FindAsync(id);
            db.Comments.Remove(comment);
            await db.SaveChangesAsync();
            //return RedirectToAction("Index");
            return RedirectToAction("Details", "Ticket", new { id = comment.TicketID });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}