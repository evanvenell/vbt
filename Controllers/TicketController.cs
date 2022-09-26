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

namespace BugTrackerApplication.Controllers
{
    public class TicketController : Controller
    {
        private ApplicationContext db = new ApplicationContext();

        // GET: Ticket
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic,Demo_Admin,Demo_Developer,Demo_ProjectManager,Demo_Submitter")]
        public ActionResult Index(string sortOrder, string searchString, string commentString)
        {
            ViewBag.IDSortParm = sortOrder == "ID" ? "id_desc" : "ID";
            ViewBag.TitleSortParm = String.IsNullOrEmpty(sortOrder) ? "titleSort_desc" : "";
            ViewBag.DevSortParm = String.IsNullOrEmpty(sortOrder) ? "devSort_desc" : "";
            ViewBag.StatSortParm = String.IsNullOrEmpty(sortOrder) ? "statSort_desc" : "";
            ViewBag.TypeSortParm = String.IsNullOrEmpty(sortOrder) ? "typeSort_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.ProjSortParm = String.IsNullOrEmpty(sortOrder) ? "projSort_desc" : "";
            var tickets = from s in db.Tickets
                          select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                tickets = tickets.Where(s => s.TicketTitle.Contains(searchString)
                                    || s.TicketDescription.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "titleSort_desc":
                    tickets = tickets.OrderByDescending(s => s.TicketTitle);
                    break;
                case "ID":
                    tickets = tickets.OrderBy(s => s.TicketID);
                    break;
                case "id_desc":
                    tickets = tickets.OrderByDescending(s => s.TicketID);
                    break;
                case "devSort_desc":
                    tickets = tickets.OrderByDescending(s => s.Project.AssignedDeveloper);
                    break;
                case "statSort_desc":
                    tickets = tickets.OrderByDescending(s => s.TicketStatus);
                    break;
                case "typeSort_desc":
                    tickets = tickets.OrderByDescending(s => s.TicketType);
                    break;
                case "Date":
                    tickets = tickets.OrderBy(s => s.Created);
                    break;
                case "date_desc":
                    tickets = tickets.OrderByDescending(s => s.Created);
                    break;
                case "projSort_desc":
                    tickets = tickets.OrderByDescending(s => s.Project.ProjectName);
                    break;
                default:
                    tickets = tickets.OrderBy(s => s.Created);
                    break;
            }

            //-- old code --//
            
            return View(tickets.ToList());

        }

        // GET: Ticket/Details/5
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic,Demo_Admin,Demo_Developer,Demo_ProjectManager,Demo_Submitter")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }


            TempData["AddComment"] = id;
            TempData["AddAttachment"] = id;

            var userId = User.Identity.GetUserId();
            var user = db.Users.Where(u => u.Id == userId).FirstOrDefault();
            var userName = user.FullName;
            TempData["AuthorID"] = userName;

            


            return View(ticket);
        }

        // GET: Ticket/Create
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic,Demo_Admin,Demo_Developer,Demo_ProjectManager,Demo_Submitter")]
        public ActionResult Create()
        {
            PopulateProjectsDropDownList();
            // auto-assign the TicketID variable.
            var queryTicketIds = db.Tickets.Select(i => i.TicketID).ToList();
            Random r = new Random();
            int Idnum = r.Next(1000, 10000);
            
            if (queryTicketIds.Any().Equals(Idnum))
            {
                Random newR = new Random();
                Idnum = newR.Next();
            }
            else
            {
                TempData["NewID"] = Idnum;
                ViewBag.TicketID = TempData["NewID"];
            }
            // ./auto assign TicketID //
            return View();
        }


        // POST: Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic")]
        public ActionResult Create([Bind(Include = "TicketID, TicketDescription, TicketStatus, TicketType, TicketPriority, AssociatedProject, TicketTitle, Created, Updated, ProjectID")] Ticket ticket)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Tickets.Add(ticket);
                    
                    ticket.Updated = System.DateTime.Now;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment the dex variable and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, please contact your System Administrator.");
            }
            PopulateProjectsDropDownList(ticket.ProjectID);
            return View(ticket);
        }

       

        // GET: Ticket/Edit/5
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic,Demo_Admin,Demo_Developer,Demo_ProjectManager,Demo_Submitter")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            PopulateProjectsDropDownList(ticket.ProjectID);
            return View(ticket);
        }

        

        // POST: Ticket/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic")]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            

            var ticketToUpdate = db.Tickets.Find(id);
            if (TryUpdateModel(ticketToUpdate, "",
                new string[] { "TicketTitle", "TicketDescription", "AssociatedProject", "Developer", "TicketStatus", "TicketType", "Created", "Updated", "TicketPriority", "ProjectID" }))
            {
                try
                {
                    ticketToUpdate.Updated = System.DateTime.Now;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment the dex variable and add a line to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, please contact your System Administrator.");
                }
            }
            PopulateProjectsDropDownList(ticketToUpdate.ProjectID);
            return View(ticketToUpdate);
        }

        //  GET: Ticket/AddDeveloperToTicket
        //
        [AllowAnonymous]
        public ActionResult AddDeveloperToTicket(int? id, int? developerId, string UserName)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets
                .Include(i => i.Developers)
                .Where(i => i.TicketID == id)
                .Single();
            Developer developer = db.Developers
                .Include(i => i.Tickets)
                .Where(i => i.ID == developerId)
                .Single();

            PopulateAssignedTicketDeveloperData(ticket);

            if (developer == null)
            {
                return HttpNotFound();
            }
            return View();

        }

        private void PopulateAssignedTicketDeveloperData(Ticket ticket)
        {
            var allDevelopers = db.Developers;
            var ticketDevelopers = new HashSet<int>(ticket.Developers.Select(c => c.ID));
            var viewModel = new List<AssignedTicketDevelopers>();
            foreach (var developer in allDevelopers)
            {
                viewModel.Add(new AssignedTicketDevelopers
                {
                    DeveloperID = developer.ID,
                    FirstMidName = developer.FirstMidName,
                    LastName = developer.LastName,
                    Assigned = ticketDevelopers.Contains(ticket.TicketID)
                });
            }
            ViewBag.TicketDevelopers = viewModel;
        }

        private void PopulateProjectsDropDownList(object selectedProject = null)
        {
            var projectsQuery = from d in db.Projects
                                orderby d.ProjectName
                                select d;
            ViewBag.ProjectID = new SelectList(projectsQuery, "ProjectID", "ProjectName", selectedProject);
        }


        // GET: Ticket/Delete/5
        [AuthLog(Roles = "Admin,Developer,Project Manager,Demo_Admin,Demo_Developer,Demo_ProjectManager,Demo_Submitter")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Ticket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,Developer,Project Manager")] //NOTE: I think customers should have the capability to REQUEST that a dev or PM CLOSE the ticket but NOT DELETE it. -EV 
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
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
