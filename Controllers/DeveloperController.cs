using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTrackerApplication.DAL;
using BugTrackerApplication.Models;
using BugTrackerApplication.ViewModels;
using System.Data.Entity.Infrastructure;
using BugTrackerApplication.Filters;
using BugTrackerApplication.CustomFilters;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Globalization;
using System.Security.Claims;
using System.Data.Sql;
using System.Data.SqlClient;

namespace BugTrackerApplication.Controllers
{
    public class DeveloperController : Controller
    {
        
        private ApplicationContext db = new ApplicationContext();

        // GET: Developer
        [AuthLog(Roles = "Admin,Developer,Project Manager,Demo_Admin,Demo_Developer,Demo_ProjectManager")]
        public ActionResult Index(int? id, int? ticketID)
        {
            var viewModel = new DeveloperIndexData();
            viewModel.Developers = db.Developers
                .Include(i => i.Tickets)
                .Include(i => i.Tickets.Select(c => c.Project))
                .OrderBy(i => i.LastName);

            if (id != null)
            {
                ViewBag.DeveloperID = id.Value;
                viewModel.Tickets = viewModel.Developers.Where(
                    i => i.ID == id.Value).Single().Tickets;
            }

            if (ticketID != null)
            {
                ViewBag.TicketID = ticketID.Value;
                // Lazy loading
                //viewModel.Enrollments = viewModel.Tickets.Where(
                //    x => x.TicketID == ticketID).Single().Enrollments;
                var selectedTicket = viewModel.Tickets.Where(x => x.TicketID == ticketID).Single();
                db.Entry(selectedTicket).Collection(x => x.Enrollments).Load();
                foreach (Enrollment enrollment in selectedTicket.Enrollments)
                {
                    db.Entry(enrollment).Reference(x => x.Submitter).Load();
                }

                viewModel.Enrollments = selectedTicket.Enrollments;

            }

            return View(viewModel);

        }

        // GET: Developer/Details/5
        [AuthLog(Roles = "Admin,Developer,Project Manager,Demo_Admin,Demo_Developer,Demo_ProjectManager")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Developer developer = db.Developers.Find(id);
            if (developer == null)
            {
                return HttpNotFound();
            }
            return View(developer);
        }

        // GET: Developer/Create
        [AuthLog(Roles = "Admin,DevSelfModify,Demo_Admin,Demo_Developer")] //NOTE: Developers and ProjectManagers will have the ability to create themselves but not others. -EV 08/11/2021...
        public ActionResult Create()
        {
            var developer = new Developer();
            developer.Tickets = new List<Ticket>();
            PopulateAssignedTicketData(developer);

            // Grab the UserID from the ManageController for Developer Creation to link to User:
            ViewBag.UserID = TempData["NewDeveloper"];
            //--------//
            return View();
        }

        // POST: Developer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,DevSelfModify")]
        public ActionResult Create([Bind(Include = "ID,LastName,FirstMidName,EmailAddress,HireDate,RoleAssignment,UserId")] Developer developer, string[] selectedTickets)
        {
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var userId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Where(u => u.Id.Equals(userId)).FirstOrDefault();

            if (selectedTickets != null)
            {
                developer.Tickets = new List<Ticket>();
                foreach (var ticket in selectedTickets)
                {
                    var ticketToAdd = db.Tickets.Find(int.Parse(ticket));
                    developer.Tickets.Add(ticketToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                
                db.Developers.Add(developer);
                db.SaveChanges();
                user.DeveloperID = developer.ID;
                // Check for the DevSelfModify Role, IF Found, THEN Remove it upon user creation...
                if (userManager.IsInRole(user.Id, "DevSelfModify"))
                {
                    userManager.RemoveFromRole(user.Id, "DevSelfModify");
                    // Re-isue cookie to signed in user to reflect updated role for access.
                    //SignInManager.SignIn(user, false, false); //NOTE: This role is removed upon user signin next time. 
                    // Add the new DeveloperID to the Identity User table entry...
                    
                    //-------//

                    // Redirect back to UserProfile
                    return RedirectToAction("Index", "Manage", new { id = user.Id });
                }
                
                return RedirectToAction("Index");
            }
            // Grab the UserID from the ManageController for Developer Creation to link to User:
            ViewBag.UserID = TempData["NewDeveloper"];
            //--------//
            PopulateAssignedTicketData(developer);
            return View(developer);
        }

        // GET: Developer/Edit/5
        [AuthLog(Roles = "Admin,DevSelfModify,Demo_Admin,Demo_Developer")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Developer developer = db.Developers
                .Include(i => i.Tickets)
                .Where(i => i.ID == id)
                .Single();
            PopulateAssignedTicketData(developer);
            if (developer == null)
            {
                return HttpNotFound();
            }
            return View(developer);
        }

        //-- PopulateAssignedTicketData --//
        private void PopulateAssignedTicketData(Developer developer)
        {
            var allTickets = db.Tickets;
            var developerTickets = new HashSet<int>(developer.Tickets.Select(c => c.TicketID));
            var viewModel = new List<AssignedTicketData>();
            foreach (var ticket in allTickets)
            {
                viewModel.Add(new AssignedTicketData
                {
                    TicketID = ticket.TicketID,
                    Title = ticket.TicketTitle,
                    Assigned = developerTickets.Contains(ticket.TicketID)
                });
            }
            ViewBag.Tickets = viewModel;
        }
        //------//


        // POST: Developer/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,DevSelfModify")]
        public ActionResult EditPost(int? id, string[] selectedTickets)
        {
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var userId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Where(u => u.Id.Equals(userId)).FirstOrDefault();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var developerToUpdate = db.Developers
                //.Include(i => i.RoleAssignment)
                .Include(i => i.Tickets)
                .Where(i => i.ID == id)
                .Single();

            if (TryUpdateModel(developerToUpdate, "",
                new string[] { "LastName", "FirstMidName", "HireDate", "RoleAssignment" }))
            {
                try
                {
                    
                    //add the httpGET code from above...
                    UpdateDeveloperTickets(selectedTickets, developerToUpdate);

                    db.SaveChanges();
                    if (userManager.IsInRole(user.Id, "DevSelfModify"))
                    {
                        userManager.RemoveFromRole(user.Id, "DevSelfModify");
                        // Re-isue cookie to signed in user to reflect updated role for access.
                        //SignInManager.SignIn(user, false, false); //NOTE: This role is removed upon user signin next time. 
                        // Add the new DeveloperID to the Identity User table entry...

                        //-------//

                        // Redirect back to UserProfile
                        return RedirectToAction("Index", "Manage", new { id = user.Id });
                    }

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, please contact your System Administrator.");
                }
            }
            PopulateAssignedTicketData(developerToUpdate);

            return View(developerToUpdate);
        }

        //-- UpdateDeveloperTickets --//
        private void UpdateDeveloperTickets(string[] selectedTickets, Developer developerToUpdate)
        {
            if (selectedTickets == null)
            {
                developerToUpdate.Tickets = new List<Ticket>();
                return;
            }

            var selectedTicketsHS = new HashSet<string>(selectedTickets);
            var developerTickets = new HashSet<int>
                (developerToUpdate.Tickets.Select(c => c.TicketID));
            foreach (var ticket in db.Tickets)
            {
                if (selectedTicketsHS.Contains(ticket.TicketID.ToString()))
                {
                    if (!developerTickets.Contains(ticket.TicketID))
                    {
                        developerToUpdate.Tickets.Add(ticket);
                    }
                }
                else
                {
                    if (developerTickets.Contains(ticket.TicketID))
                    {
                        developerToUpdate.Tickets.Remove(ticket);
                    }
                }
            }
        }
        //------//

        // GET: Developer/Delete/5
        [AuthLog(Roles = "Admin,Demo_Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Developer developer = db.Developers.Find(id);
            if (developer == null)
            {
                return HttpNotFound();
            }
            return View(developer);
        }

        // POST: Developer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Developer developer = db.Developers
                //.Include(i => i.RoleAssignment)
                .Where(i => i.ID == id)
                .Single();

            db.Developers.Remove(developer);

            var project = db.Projects
                .Where(d => d.DeveloperID == id)
                .SingleOrDefault();
            if (project != null)
            {
                project.DeveloperID = null;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
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
