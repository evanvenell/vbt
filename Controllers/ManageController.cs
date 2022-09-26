using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BugTrackerApplication.Models;
using BugTrackerApplication.DAL;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Globalization;
using System.Security.Claims;
using System.Data;
using System.Data.Entity;
using System.Net;
using BugTrackerApplication.ViewModels;
using BugTrackerApplication.Filters;
using BugTrackerApplication.CustomFilters;
using System.Data.Entity.Infrastructure;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;


namespace BugTrackerApplication.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;


        ApplicationContext context;

        public ManageController()
        {
            //ApplicationContext context;
            context = new ApplicationContext();
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic,Demo_Admin,Demo_Developer,Demo_ProjectManager,Demo_Submitter")]
        public async Task<ActionResult> Index(ManageMessageId? message, Developer developer, Submitter submitter)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                //--New User Code --//
                //: message == ManageMessageId.FirstTimeUser ? "Welcome, your account needs a few more steps to be fully set up."
                : "";
            // Grab the UserData 
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var userId = User.Identity.GetUserId();
            ApplicationUser user = context.Users.Where(u => u.Id.Equals(userId)).FirstOrDefault();

            //---------------//
            //-- Employee --//
            if (user.UserType == UserType.Employee)
            {
               var queryDeveloperUserID = context.Developers.Where(u => u.UserId.Equals(
                          user.Id, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
                // Check for Existing Developer Entry, IF NOT found, THEN Create one
                if (queryDeveloperUserID == null)
                {
                    // Add role to user for DevSelfModify

                    userManager.AddToRole(user.Id, "DevSelfModify");
                    // Re-isue cookie to signed in user to reflect updated role for access.
                    SignInManager.SignIn(user, false, false);
                    // Create the TempData for auto import of pre filled UserId value for Developer entry creation.
                    TempData["NewDeveloper"] = user.Id;
                    // redirect to developer creation in create view...(See Developer Controller - Create method next for the removal of the role).
                    return RedirectToAction("Create", "Developer", new { id = developer.ID });
                    //return RedirectToAction("Create", "Developer", new { id = queryDeveloperUserID.ID });
                    //NOTE: The issue with my Developer piece is the fact that the developer variable is only being pulled form the above callout in the method declaration.
                    //      ^^^^... I need to create another var after this to create an associated between the User and the Developer. -EV 08/18/2021...
                }
                //else //NOTE: This is the pieces that messes up the ProfileSettings. -EV 08/18/2021...
                //{
                //    var developerUserProfile = context.AppUsers.Where(u => u.DeveloperID.Equals(
                //        user.DeveloperID)).SingleOrDefault();
                //    //return View(developerUserProfile);

                //    //return View(developerViewModel);
                //    return View();
                //}

            }
            //-- ./Employee --//

            //-- Customer --//
            if (user.UserType == UserType.Customer)
            {
                var querySubmitterUserID = context.Submitters.Where(u => u.UserId.Equals(
                          user.Id, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
                // Check for Existing Submitter Entry, IF NOT found, THEN Create one
                if (querySubmitterUserID == null)
                {
                    // Add role to user for SubSelfModify

                    userManager.AddToRole(user.Id, "SubSelfModify");
                    // Re-isue cookie to signed in user to reflect updated role for access.
                    SignInManager.SignIn(user, false, false);
                    // Create the TempData for auto import of pre filled UserId value for Submitter entry creation.
                    TempData["NewSubmitter"] = user.Id;
                    // redirect to submitter creation in create view...(See Submitter Controller - Create method next for the removal of the role).
                    return RedirectToAction("Create", "Submitter", new { id = submitter.ID });
                }
            }
            //-- ./Customer --//
            //var assoicatedDevProfile = context.AppUsers.Where(u => u.DeveloperID.Equals(
            //    developer.ID)).SingleOrDefault();
            var associatedDevProfile = context.Developers.Where(u => u.ID.Equals(
                user.DeveloperID)).SingleOrDefault();
            var associatedSubProfile = context.Submitters.Where(u => u.ID.Equals(
                user.SubmitterID)).SingleOrDefault();
        if (associatedDevProfile != null)
            {
                var devViewModel = new IndexViewModel
                {
                    HasPassword = HasPassword(),
                    PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                    TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                    Logins = await UserManager.GetLoginsAsync(userId),
                    BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                    //------//
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    JobTitle = user.JobTitle,
                    Location = user.Location,
                    EducationOrExperience = user.EducationOrExperience,
                    Skills = user.Skills,
                    Notes = user.Notes,
                    //-- Developer Details --//
                    DevFirstName = associatedDevProfile.FirstMidName,
                    DevLastName = associatedDevProfile.LastName,
                    DevEmail = associatedDevProfile.EmailAddress,
                    //-- ./Developer Details --//
                    //-- Query Statistics for Tickets, Projects, and Comments --//

                    //-- ./Query Statistics for Tickets, Projects, and Comments --//
                };
                if (devViewModel != null)
                {
                    // Store the associated developer profile in a ViewBag variable for later View control. 
                    ViewBag.UserDeveloperID = associatedDevProfile.ID;
                    //ViewBag.UserSubmitterID = associatedSubProfile.ID;
                    return View(devViewModel);
                }
            }

        if (associatedSubProfile != null)
            {
                var subViewModel = new IndexViewModel
                {
                    HasPassword = HasPassword(),
                    PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                    TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                    Logins = await UserManager.GetLoginsAsync(userId),
                    BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                    //------//
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    JobTitle = user.JobTitle,
                    Location = user.Location,
                    EducationOrExperience = user.EducationOrExperience,
                    Skills = user.Skills,
                    Notes = user.Notes,
                    //-- Developer Details --//
                    SubFirstName = associatedSubProfile.FirstMidName,
                    SubLastName = associatedSubProfile.LastName,
                    SubEmail = associatedSubProfile.EmailAddress,
                    //-- ./Developer Details --//
                    //-- Query Statistics for Tickets, Projects, and Comments --//

                    //-- ./Query Statistics for Tickets, Projects, and Comments --//
                };
                if (subViewModel != null)
                {
                    // Store the associated developer profile in a ViewBag variable for later View control. 
                    //ViewBag.UserDeveloperID = associatedDevProfile.ID;
                    ViewBag.UserSubmitterID = associatedSubProfile.ID;
                    return View(subViewModel);
                }
            }
            return View();
            
        }

        public FileContentResult Photo(string userId)
        {
            var db = HttpContext.GetOwinContext().Get<ApplicationContext>();

            var user = db.Users.Where(u => u.Id == userId).FirstOrDefault();

            return new FileContentResult(user.ProfilePicture, "image/jpeg");
        }

        //
        //  GET: Manage/ProfilePhoto
        [HttpGet]
        public ActionResult ProfilePhoto()
        {
            ViewBag.Message = "Update your profile photo.";
            return View();
        }

        //
        //  POST: Manage/ProfilePhoto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProfilePhoto(HttpPostedFileBase ProfilePhoto)
        {
            if (ModelState.IsValid)
            {
                //  Get EF Database.
                //var db = HttpContext.GetOwinContext().Get<ApplicationContext>();

                //  Find the user. 
                var userId = User.Identity.GetUserId();
                var user = context.Users.Where(u => u.Id == userId).FirstOrDefault();

                //  Convert image stream to byte array.
                byte[] image = new byte[ProfilePhoto.ContentLength];
                ProfilePhoto.InputStream.Read(image, 0, Convert.ToInt32(ProfilePhoto.ContentLength));
                ViewBag.PreviewImage = image;
                user.ProfilePicture = image;

                //  Save Changes to db.
                context.SaveChanges();

                return RedirectToAction("Index", "Manage");
            }
            else
            {
                ViewBag.Message = "Error saving changes.";
                return View();
            }
            //return View();
            
        }

        //
        //  GET: Developer/Create (Limited)
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic,Demo_Admin,Demo_Developer,Demo_ProjectManager,Demo_Submitter")]
        public ActionResult CreateDeveloper()
        {
            var developer = new Developer();
            return View();
        }

        //
        //  POST: Developer/Create (Limited)
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic")]
        public ActionResult Create([Bind(Include = "ID,LastName,FirstMidName,EmailAddress,HireDate,RoleAssignment,UserId")] Developer developer)
        {
            var context = new ApplicationContext();
            if (ModelState.IsValid)
            {
                context.Developers.Add(developer);
                context.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(developer);
        }

        //
        // GET: Manage/_UserSettingsPartial
        //public PartialViewResult Settings()
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic,Demo_Admin,Demo_Developer,Demo_ProjectManager,Demo_Submitter")]
        public PartialViewResult UserSettings()
        {
            return PartialView("_UserSettingsPartial");

        }

        //
        //  GET: Manage/_ProfileSettingsPartial
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic,Demo_Admin,Demo_Developer,Demo_ProjectManager,Demo_Submitter")]
        public PartialViewResult ProfileSettings()
        {
            return PartialView("_ProfileSettingsPartial");
        }

        //
        //  POST: Manage/EditSettings (Settings Tab)
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSettings(string UserName, string RoleName)
        {

            return View();
        }

        //
        // POST: Manage/_UserSettingsPartial
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic")]
        [HttpPost]
        public ActionResult UserSettings(string UserName, string RoleName)
        {
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var userId = User.Identity.GetUserId();
            ApplicationUser user = context.Users.Where(u => u.Id.Equals(userId)).FirstOrDefault();

            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var settingsToUpdate = context.Users.Find(userId);

            //var settingsToUpdate = context.Users
            //    .Include(i => i.Id)
            //    .Where(i => i.Id == userId)
            //    .Single();

            if (TryUpdateModel(settingsToUpdate, "",
                new string[] { "FirstName", "LastName", "JobTitle", "Location", "Email", "EducationOrExperience", "Skills", "Notes", "PhoneNumber" }))
            {
                try
                {
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Error: Unable to update User Settings. Please try again and if the problem persists, please contact your System Administrator.");
                }
            }
            return View();
        }

        //
        //  POST Manage/_UserSettingsPartial
        [AuthLog(Roles = "Admin,Developer,Project Manager,Basic")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProfileSettings(string UserName, string RoleName)
        {
            // Grab the UserData 
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var userId = User.Identity.GetUserId();
            ApplicationUser user = context.Users.Where(u => u.Id.Equals(userId)).FirstOrDefault();

            if (ModelState.IsValid)
            {
                //-- Employee --//
                if (user.UserType == UserType.Employee)
                {
                    // Developer Vars
                    var associatedDevProfile = context.Developers.Where(u => u.ID.Equals(
                        user.DeveloperID)).SingleOrDefault();
                    var devSettingsToUpdate = context.Developers.Find(associatedDevProfile.ID);
                    // Add User to role DevSelfModify.
                    userManager.AddToRole(user.Id, "DevSelfModify");
                    // Refresh Authentication Cookie to not have to log in again.
                    SignInManager.SignIn(user, false, false);
                    // Create the ViewBag or take user to EDIT screen for associated developer profile.
                    //ViewBag.UserDeveloperID = associatedDevProfile.ID;
                    return RedirectToAction("Edit", "Developer", new { id = associatedDevProfile.ID });
                }
                //-- ./Employee --//

                //-- Customer --//
                if (user.UserType == UserType.Customer)
                {
                    // Submitter Vars
                    var associatedSubProfile = context.Submitters.Where(u => u.ID.Equals(
                        user.SubmitterID)).SingleOrDefault();
                    var subSettingsToUpdate = context.Submitters.Find(associatedSubProfile.ID);
                    // Add User to role SubSelfModify.
                    userManager.AddToRole(user.Id, "SubSelfModify");
                    // Refresh Authentication Cookie to not have to log in again.
                    SignInManager.SignIn(user, false, false);
                    // Create the ViewBag or take user to EDIT screen for associated submitter profile.
                    //ViewBag.UserDeveloperID = associatedDevProfile.ID;
                    return RedirectToAction("Edit", "Submitter", new { id = associatedSubProfile.ID });
                }
                //-- ./Customer --//
            }
            return View();

        }

        //--- Original Index Code - Working --//
        //public async Task<ActionResult> Index(ManageMessageId? message)
        //{
        //    ViewBag.StatusMessage =
        //        message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
        //        : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
        //        : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
        //        : message == ManageMessageId.Error ? "An error has occurred."
        //        : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
        //        : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
        //        //--New User Code --//
        //        //: message == ManageMessageId.FirstTimeUser ? "Welcome, your account needs a few more steps to be fully set up."
        //        : "";

        //    var userId = User.Identity.GetUserId();
        //    var model = new IndexViewModel
        //    {
        //        HasPassword = HasPassword(),
        //        PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
        //        TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
        //        Logins = await UserManager.GetLoginsAsync(userId),
        //        BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
        //        //FirstTimeUser = await UserManager.UserLockoutEnabledByDefault(userId)

        //    };
        //    return View(model);
        //}



        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin")]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        [AuthLog(Roles = "Admin,DevSelfModify,SubSelfModify,Demo_Admin")]
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,DevSelfModify,SubSelfModify")]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin")]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin")]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        [AuthLog(Roles = "Admin,DevSelfModify,SubSelfModify,Demo_Admin")]
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,DevSelfModify,SubSelfModify")]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,DevSelfModify,SubSelfModify")]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        [AuthLog(Roles = "Admin,DevSelfModify,SubSelfModify,Demo_Admin")]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,DevSelfModify,SubSelfModify")]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        [AuthLog(Roles = "Admin,DevSelfModify,SubSelfModify,Demo_Admin")]
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,DevSelfModify,SubSelfModify")]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        [AuthLog(Roles = "Admin,DevSelfModify,SubSelfModify_Demo_Admin")]
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin")]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        [AuthLog(Roles = "Admin")]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        //
        // GET: /Account/Profile
        //public ActionResult UserProfile()
        //{
        //    return View();
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error,
            //New User Code //
            //FirstTimeUser,
        }

        #endregion
    }
}