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
    public class ProjectController : Controller
    {
        private ApplicationContext db = new ApplicationContext();

        // GET: Project
        //NOTE: This is how you authorize multiple roles for one method/action (Make sure to not add a space between the comma after the first roleName and the lastRolename.
        [AuthLog(Roles = "Admin,Project Manager,Developer,Demo_Admin,Demo_Developer,Demo_ProjectManager")] 
        public async Task<ActionResult> Index(int? id, int? ticketID, int? developerID, string sortOrder, string searchString)
        {

            var viewModel = new ProjectDetailData();
            viewModel.Developers = db.Developers
                .Include(i => i.FullName)
                .Include(i => i.Tickets.Select(c => c.Project))
                .OrderBy(i => i.FirstMidName);

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

                // Explicit Loading
                var selectedTicket = viewModel.Tickets.Where(x => x.TicketID == ticketID).Single();
                db.Entry(selectedTicket).Collection(x => x.Enrollments).Load();
                foreach (Enrollment enrollment in selectedTicket.Enrollments)
                {
                    db.Entry(enrollment).Reference(x => x.Submitter).Load();
                }

                viewModel.Enrollments = selectedTicket.Enrollments;
            }

            if (developerID != null)
            {
                ViewBag.DeveloperID = id.Value;
                viewModel.Developers = viewModel.Tickets.Where(
                    i => i.TicketID == developerID.Value).Single().Developers;
            }




            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.DescriptionSortParm = sortOrder == "Description" ? "description_desc" : "Description";
            ViewBag.DevSortParm = sortOrder == "Assigned Developer" ? "assignedDeveloper_desc" : "Assigned Developer";
            
            ViewBag.BudgetSortParm = sortOrder == "Budget" ? "budget_desc" : "Budget";
            


            var projects = from s in db.Projects
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(s => s.ProjectName.Contains(searchString)
                                        || s.ProjectDescription.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    projects = projects.OrderByDescending(s => s.ProjectName);
                    break;
                case "Description":
                    projects = projects.OrderBy(s => s.ProjectDescription);
                    break;
                case "description_desc":
                    projects = projects.OrderByDescending(s => s.ProjectDescription);
                    break;
                case "Assigned Developer":
                    projects = projects.OrderBy(s => s.AssignedDeveloper);
                    break;
                case "assignedDeveloper_desc":
                    projects = projects.OrderByDescending(s => s.AssignedDeveloper);
                    break;
                case "Budget":
                    projects = projects.OrderBy(s => s.Budget);
                    break;
                case "budget_desc":
                    projects = projects.OrderByDescending(s => s.Budget);
                    break;
                case "Date":
                    projects = projects.OrderBy(s => s.StartDate);
                    break;
                case "date_desc":
                    projects = projects.OrderByDescending(s => s.StartDate);
                    break;
                default:
                    projects = projects.OrderBy(s => s.StartDate);
                    break;
            }

            return View(await projects.ToListAsync());
        }

        

        // GET: Project/Details/5
        //public async Task<ActionResult> Details(int? id)

        [AuthLog(Roles = "Admin,Developer,Project Manager,Demo_Admin,Demo_Developer,Demo_ProjectManager")]
        public async Task<ActionResult> Details(int? id, int? ticketID, Developer developer, string[] selectedTicket)
        {
            

            //-- DetailsGET old code --//
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Commenting out original code to show how to use a raw SQL query.
            //Project project = await db.Projects.FindAsync(id); //NOTE: Here's where the method gets excuted asynchronously. -EV 01/29/2021...

            // Create and execute raw SQL query.
            string query = "SELECT * FROM Project WHERE ProjectID = @p0";
            Project project = await db.Projects.SqlQuery(query, id).SingleOrDefaultAsync();

            


            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Project/Create
        [AuthLog(Roles = "Admin,Developer,Project Manager,Demo_Admin,Demo_Developer,Demo_ProjectManager")]
        public ActionResult Create()
        {
            //ViewBag.DeveloperID = new SelectList(db.Developers, "ID", "LastName"); //NOTE: Change all of the rest below. -EV 01/29/2021...
            ViewBag.DeveloperID = new SelectList(db.Developers, "ID", "FullName");
            return View();
        }

        // POST: Project/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,Developer,Project Manager")]
        public async Task<ActionResult> Create([Bind(Include = "ProjectID,ProjectName,ProjectDescription,AssignedDeveloper,Budget,StartDate,DeveloperID")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.DeveloperID = new SelectList(db.Developers, "ID", "FullName", project.DeveloperID);
            return View(project);
        }

        // GET: Project/Edit/5
        [AuthLog(Roles = "Admin,Developer,Project Manager,Demo_Admin,Demo_Developer,Demo_ProjectManager")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeveloperID = new SelectList(db.Developers, "ID", "FullName", project.DeveloperID);
            return View(project);
        }

        // POST: Project/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,Developer,Project Manager")]
        public async Task<ActionResult> Edit(int? id, byte[] rowVersion)
        {
            string[] fieldsToBind = new string[] { "ProjectName", "ProjectDescription", "Budget", "StartDate", "DeveloperID", "RowVersion" };

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var projectToUpdate = await db.Projects.FindAsync(id);
            if (projectToUpdate == null)
            {
                Project deletedProject = new Project();
                TryUpdateModel(deletedProject, fieldsToBind);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The Project was deleted by another user.");
                ViewBag.DeveloperID = new SelectList(db.Developers, "ID", "FullName", deletedProject.DeveloperID);
                return View(deletedProject);
            }

            if (TryUpdateModel(projectToUpdate, fieldsToBind))
            {
                try
                {
                    db.Entry(projectToUpdate).OriginalValues["RowVersion"] = rowVersion;
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var clientValues = (Project)entry.Entity;
                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The Project was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Project)databaseEntry.ToObject();

                        if (databaseValues.ProjectName != clientValues.ProjectName)
                            ModelState.AddModelError("ProjectName", "Current value: "
                                + databaseValues.ProjectName);
                        if (databaseValues.Budget != clientValues.Budget)
                            ModelState.AddModelError("Budget", "Current value: "
                                + String.Format("{0:c}", databaseValues.Budget));
                        if (databaseValues.StartDate != clientValues.StartDate)
                            ModelState.AddModelError("StartDate", "Current value: "
                                + String.Format("{0:d}", databaseValues.StartDate));
                        if (databaseValues.DeveloperID != clientValues.DeveloperID)
                            ModelState.AddModelError("DeveloperID", "Current value: "
                                + db.Developers.Find(databaseValues.DeveloperID).FullName);
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                            + "was modified by another user after you got the original value. The "
                            + "edit operation was canceled and the current values in the database "
                            + "have been displayed. If you still want to edit this record, click "
                            + "the Save button again. Otherwise click the Back to List hyperlink.");
                        projectToUpdate.RowVersion = databaseValues.RowVersion;
                    }
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, please contact your System Administrator.");
                }
            }
            ViewBag.DeveloperID = new SelectList(db.Developers, "ID", "FullName", projectToUpdate.DeveloperID);
            return View(projectToUpdate);
        }


        // GET: Project/Delete/5
        [AuthLog(Roles = "Admin,Project Manager,Demo_Admin,Demo_Developer,Demo_ProjectManager")]
        public async Task<ActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("Index");
                }
                return HttpNotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete "
                    + "was modifed by another user after you got the original values. "
                    + "The delete operation has been canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(project);
        }

        // POST: Project/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,Project Manager")]
        public async Task<ActionResult> Delete(Project project)
        {
            try
            {
                db.Entry(project).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = project.ProjectID });
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to delete. Try again, and if the problem persists please contact your System Administrator.");
                return View(project);
            }
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
