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
using PagedList;
using System.Data.Entity.Infrastructure;
using BugTrackerApplication.Filters;
using BugTrackerApplication.CustomFilters;
using BugTrackerApplication.ViewModels;
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
    public class SubmitterController : Controller
    {
        private ApplicationContext db = new ApplicationContext();

        // GET: Submitter
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic,Demo_Admin,Demo_Developer,Demo_ProjectManager,Demo_Submitter")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.FirstNameSortParm = String.IsNullOrEmpty(sortOrder) ? "firstName_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) ? "email" : "";
            ViewBag.EmailSortDescParm = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "";
            ViewBag.RoleSortParm = String.IsNullOrEmpty(sortOrder) ? "role_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var submitters = from s in db.Submitters
                        select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                submitters = submitters.Where(s => s.LastName.Contains(searchString)
                                || s.FirstMidName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    submitters = submitters.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    submitters = submitters.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    submitters = submitters.OrderByDescending(s => s.EnrollmentDate);
                    break;
                case "firstName":
                    submitters = submitters.OrderBy(s => s.FirstMidName);
                    break;
                case "firstName_desc":
                    submitters = submitters.OrderByDescending(s => s.FirstMidName);
                    break;
                case "email":
                    submitters = submitters.OrderBy(s => s.EmailAddress);
                    break;
                case "email_desc":
                    submitters = submitters.OrderByDescending(s => s.EmailAddress);
                    break;
                default:
                    submitters = submitters.OrderBy(s => s.FirstMidName);
                    break;
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(submitters.ToPagedList(pageNumber, pageSize));
        }

        // GET: Submitter/Details/5
        [AuthLog(Roles = "Admin,Developer,Project Manager,Demo_Admin,Demo_Developer,Demo_ProjectManager")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Submitter submitter = db.Submitters.Find(id);
            if (submitter == null)
            {
                return HttpNotFound();
            }
            return View(submitter);
        }

        // GET: Submitter/Create
        [AuthLog(Roles = "Admin,SubSelfModify,Demo_Admin")]
        public ActionResult Create()
        {
            // Grab the UserID from the ManageController for Developer Creation to link to User:
            ViewBag.UserID = TempData["NewSubmitter"];
            return View();
        }

        // POST: Submitter/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,SubSelfModify")]
        public ActionResult Create([Bind(Include = "ID,LastName,FirstMidName,EmailAddress,EnrollmentDate,UserId")] Submitter submitter)
        {
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var userId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Where(u => u.Id.Equals(userId)).FirstOrDefault();

            
            if (ModelState.IsValid)
            {
                db.Submitters.Add(submitter);
                db.SaveChanges();
                user.SubmitterID = submitter.ID;
                // Check for the SubSelfModify Role. IF Found, THEN Remove it upon user creation.
                if (userManager.IsInRole(user.Id, "SubSelfModify"))
                {
                    userManager.RemoveFromRole(user.Id, "SubSelfModify");
                    //-------//

                    // Redirect back to UserProfile.
                    return RedirectToAction("Index", "Manage", new { id = user.Id });
                }
                return RedirectToAction("Index");
            }
            // Grab the UserID from the ManageController for Submitter Creation to link to User.
            ViewBag.UserID = TempData["NewSubmitter"];
            return View(submitter);
        }

        // GET: Submitter/Edit/5
        [AuthLog(Roles = "Admin,SubSelfModify,Demo_Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Submitter submitter = db.Submitters.Find(id);
            if (submitter == null)
            {
                return HttpNotFound();
            }
            return View(submitter);
        }

        // POST: Submitter/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,SubSelfModify")]
        public ActionResult EditPost(int? id)
        {
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var userId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Where(u => u.Id.Equals(userId)).FirstOrDefault();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var submitterToUpdate = db.Submitters.Find(id);
            if (TryUpdateModel(submitterToUpdate, "",
               new string[] { "LastName", "FirstMidName", "EmailAddress", "EnrollmentDate" }))
            {
                try
                {
                    db.SaveChanges();
                    // Remove the user from the SubSelfModify Role before redirecting to UserProfile index.
                    if (userManager.IsInRole(user.Id, "SubSelfModify"))
                    {
                        userManager.RemoveFromRole(user.Id, "SubSelfModify");

                        return RedirectToAction("Index", "Manage", new { id = user.Id });
                    }

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists please see your System Administrator.");
                }
            }
            return View(submitterToUpdate);
        }

        // GET: Submitter/Delete/5
        //public ActionResult Delete(int? id)
        [AuthLog(Roles = "Admin,Demo_Admin")]
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists please see your System Administrator.";
            }
            Submitter submitter = db.Submitters.Find(id);
            if (submitter == null)
            {
                return HttpNotFound();
            }
            return View(submitter);
        }

        // POST: Submitter/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            try
            {
                Submitter submitter = db.Submitters.Find(id);
                db.Submitters.Remove(submitter);
                db.SaveChanges();
            }
            catch (RetryLimitExceededException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
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
