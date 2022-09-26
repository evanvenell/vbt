using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTrackerApplication.DAL;
using BugTrackerApplication.Models;
using BugTrackerApplication.ViewModels;
using System.Data.Entity.Infrastructure;
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

namespace BugTrackerApplication.Controllers
{
    public class CommentController : Controller
    {
        private ApplicationContext db = new ApplicationContext();

        // GET: Comment
        [AuthLog(Roles = "Admin")] 
        public async Task<ActionResult> Index()
        {
            var comments = db.Comments.Include(c => c.Ticket);
            return View(await comments.ToListAsync());
        }

        // GET: Comment/Details/5
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic,Demo_Admin,Demo_Developer,Demo_ProjectManager,Demo_Submitter")]
        public async Task<ActionResult> Details(int? id)
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

        // GET: Comment/Create
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic,Demo_Admin,Demo_Developer,Demo_ProjectManager,Demo_Submitter")]
        public ActionResult Create()
        {
            

            //Link comment to ticket //
            ViewBag.TicketID = TempData["AddComment"];
            return View();
        }

        // POST: Comment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic")]
        public async Task<ActionResult> Create([Bind(Include = "ID,TicketID,CommentTitle,Commenter,Message,Created")] Comment comment, int? ticketID)
        { //NOTE: The Demo_Roles will not have permission to actually SaveChanges() only to see the interface.


            if (ModelState.IsValid)
            {
                // Auto Assign the Commenter prop based on the matching authenticated profile to the corresponding dev or sub profile. 
                var userStore = new UserStore<ApplicationUser>(db);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var userId = User.Identity.GetUserId();
                ApplicationUser user = db.Users.Where(u => u.Id.Equals(userId)).FirstOrDefault();

                // Developer assign
                if (user.UserType == UserType.Employee)
                {
                    var queryDeveloperUserID = db.Developers.Where(u => u.UserId.Equals(
                        user.Id, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
                    if (queryDeveloperUserID != null)
                    {
                        var DeveloperName = queryDeveloperUserID.FullName;
                        comment.Commenter = DeveloperName;
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
                        comment.Commenter = SubmitterName;
                    }
                }

                // Auto assign the CreatedDate prop//
                comment.Created = System.DateTime.Now;
                db.Comments.Add(comment);
                await db.SaveChangesAsync();
                //-- Update the Updated Prop for comment ticket --//
                var ticketTimestampToUpdate = db.Tickets
                    .Where(i => i.TicketID == comment.TicketID).SingleOrDefault();
                ticketTimestampToUpdate.Updated = System.DateTime.Now;
                await db.SaveChangesAsync();

                return RedirectToAction("Details", "Ticket", new { id = comment.TicketID }); 
            }

            ViewBag.TicketID = TempData["AddComment"];

            return View(comment);
        }

       

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
