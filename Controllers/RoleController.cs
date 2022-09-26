using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity.EntityFramework;
using BugTrackerApplication.Models;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BugTrackerApplication.DAL;
using System.Data;
using System.Data.Entity;
using System.Net;
using BugTrackerApplication.ViewModels;
using System.Data.Entity.Infrastructure;
using System.Data.Sql;
using System.Data.SqlClient;
using BugTrackerApplication.Filters;
using BugTrackerApplication.CustomFilters;


namespace BugTrackerApplication.Controllers
{
    public class RoleController : Controller
    {
        //private ApplicationSignInManager _signInManager;
        //private ApplicationUserManager _userManager;

        ApplicationContext context;

        public RoleController()
        {
            context = new ApplicationContext();
        }

        //public RoleController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        //{
        //    UserManager = userManager;
        //    SignInManager = signInManager;
        //}

        //public ApplicationSignInManager SignInManager
        //{
        //    get
        //    {
        //        return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        //    }
        //    private set
        //    {
        //        _signInManager = value;
        //    }
        //}

        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}


        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        //public ActionResult Index()
        [AuthLog(Roles = "Admin,Basic")]
        public ActionResult Index(string UserName, string RoleName)
        {
            // Populate the Dropdown lists
            var context = new ApplicationContext();

            var rolelist = context.Roles.OrderBy(r => r.Name).ToList().Select(rr =>
                new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.AppRoles = rolelist;

            var userlist = context.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
                new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
            ViewBag.AppUsers = userlist;


            //-- Populate the table --//
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            var users = userManager.Users.ToList();

            var userRolesViewModel = new List<RoleManagerViewModel>();

            foreach (ApplicationUser user in users)
            {
                var thisViewModel = new RoleManagerViewModel();
                thisViewModel.UserId = user.Id;
                thisViewModel.FirstName = user.FirstName;
                thisViewModel.LastName = user.LastName;
                thisViewModel.Email = user.Email;
                thisViewModel.Roles = userManager.GetRoles(user.Id);
                userRolesViewModel.Add(thisViewModel);
            }

            return View(userRolesViewModel);
            //------------------------//
            //ViewBag.Message = "";

            //return View("Index");

        }

        [AuthLog(Roles = "Admin,Basic")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string UserName, string RoleName)
        {
            var context = new ApplicationContext();

            if (context == null)
            {
                throw new ArgumentNullException("context", "Context must not be null.");
            }

            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            //userManager.AddToRole(user.Id, RoleName);

            //Check for duplicate role.
            if (userManager.IsInRole(user.Id, RoleName))
            {
                ModelState.AddModelError("", "ERROR: Unable to modify User Role. User(s) is/are part of the selected Role.");
                //return View();
            }
            else
            {
                // Check for PENDING ROLE.
                if (userManager.IsInRole(user.Id, "Pending"))
                {
                    userManager.RemoveFromRole(user.Id, "Pending");
                    userManager.AddToRole(user.Id, RoleName);

                    // Repopulate Dropdown lists
                    var rolelistBeta = context.Roles.OrderBy(r => r.Name).ToList().Select(rr =>
                        new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                    ViewBag.AppRoles = rolelistBeta;

                    var userlistBeta = context.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
                        new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
                    ViewBag.AppUsers = userlistBeta;
                    return RedirectToAction("Index");

                }
                //-----------------------//
                // IF PENDING is Not assigned, THEN Add the selected Role.
                


                userManager.AddToRole(user.Id, RoleName);
                // Repopulate Dropdown lists
                var rolelistAlpha = context.Roles.OrderBy(r => r.Name).ToList().Select(rr =>
                    new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.AppRoles = rolelistAlpha;

                var userlistAlpha = context.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
                    new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
                ViewBag.AppUsers = userlistAlpha;
                RedirectToAction("Index");
                //--------------------------------------------------------//
            }

            ViewBag.Message = "Role added to User successfully!";

            // Repopulate Dropdown lists
            //var rolelist = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => 
            //    new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            //ViewBag.AppRoles = rolelist;
            
            //var userlist = context.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
            //    new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
            //ViewBag.AppUsers = userlist;

            //return View("Index");
            return RedirectToAction("Index");
        }

        [AuthLog(Roles = "Admin,Basic")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult GetRoles(string UserName)
        {
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                var context = new ApplicationContext();
                ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                ViewBag.RolesForThisUser = userManager.GetRoles(user.Id);

                
                // Repopulate Dropdown Lists
                var rolelist = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.AppRoles = rolelist;
                var userlist = context.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
                new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
                ViewBag.AppUsers = userlist;
                ViewBag.Message = "Roles retrieved successfully !";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
            //return View("Index");
        }

        [AuthLog(Roles = "Admin,Basic")]
        // Deleting a User from a Role
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteRoleFromUser(string UserName, string RoleName)
        {
            var account = new AccountController();
            var context = new ApplicationContext();
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var userRoleCount = userManager.GetRoles(user.Id).Count();

            if (userRoleCount == 1)
            {
                // Auto add pending role if the user has only ONE role assigned currently to keep compliant with at least one role required per user.  
                userManager.AddToRole(user.Id, "Pending");
            }

            if (userManager.IsInRole(user.Id, RoleName))
            {
                var numberOfRoles = userManager.GetRoles(user.Id).Count();
                
                    userManager.RemoveFromRole(user.Id, RoleName);
                    ViewBag.Message = "Role removed from this user successfully.";

                    // Refresh Authentication Cookie to not have to log in again.
                    //SignInManager.SignIn(user, false, false); //NOTE: THis just signs in the user I have licked, I need to fix that later. WIll come back. -EV 08/20/2021...
                // Repopulate Dropdown Lists
                var rolelistRemove = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => 
                        new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                    ViewBag.AppRoles = rolelistRemove;
                    var userlistRemove = context.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
                        new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
                    ViewBag.AppUsers = userlistRemove;

                return RedirectToAction("Index");
                
                
                //userManager.RemoveFromRole(user.Id, RoleName);
                //ViewBag.Message = "Role removed from this user successfully.";
                //return RedirectToAction("RoleAddToUser");
            }
            else
            {
                ViewBag.Message = "This user doesn't belong to the selected Role.";
            }

            // Repopulate Dropdown Lists
            var rolelist = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.AppRoles = rolelist;
            var userlist = context.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
            new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
            ViewBag.AppUsers = userlist;
            
            //return View("Index");
            return RedirectToAction("Index");
        }


        //-- Original Code --//
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult RoleAddToUser(string userName, string roleName)
        //{
        //    var context = new ApplicationContext();

        //    if (context == null)
        //    {
        //        throw new ArgumentNullException("context", "Context must not be null.");
        //    }

        //    ApplicationUser user = context.Users.Where(u => u.UserName.Equals(
        //        userName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

        //    var userStore = new UserStore<ApplicationUser>(context);
        //    var userManager = new UserManager<ApplicationUser>(userStore);
        //    userManager.AddToRole(user.Id, roleName);

        //    //ViewBag.Message = "Role created successfully !";
        //    ViewBag.Message = "User Role updates successfully!";

        //    // Repopulate Dropdown lists for Users and Roles...
        //        // roles //
        //    var roleList = context.Roles.OrderBy(r => r.Name).ToList().Select(
        //        rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
        //    ViewBag.AppRoles = roleList;
        //    //-- ./roles --//
        //        // user //
        //    var userList = context.Users.OrderBy(u => u.UserName).ToList().Select(
        //        uu => new SelectListItem { Value = uu.FullName.ToString(), Text = uu.FullName }).ToList();
        //    ViewBag.AppUsers = userList;
        //    //-- ./users --//
        //    //-- ./Repopulate Dropdown lists for Users and Roles... --//

        //    //return View("Index");
        //    return RedirectToAction("Index");
        //    //return View();
        //}



        /// <summary>
        /// Create a New role
        /// </summary>
        /// <return></return>
        [AuthLog(Roles = "Admin,Basic")]
        public ActionResult Create()
        {
            var Role = new IdentityRole();
            return View(Role);
        }

        /// <summary>
        /// Create a new role POST
        /// </summary>
        /// <param name="role"></param>
        /// <return></return>
        [AuthLog(Roles = "Admin,Basic")]
        [HttpPost]
        public ActionResult Create(IdentityRole role)
        {
            context.Roles.Add(role);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Manage Roles
        /// </summary>
        


        //-- Original Code on Controller Create --//
        // GET: Role
        //public ActionResult Index()
        //{
        //    return View();
        //}
    }
}