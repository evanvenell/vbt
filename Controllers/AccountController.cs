using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BugTrackerApplication.Models;
using BugTrackerApplication.DAL;

using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Net;
using BugTrackerApplication.ViewModels;
using System.Data.Entity.Infrastructure;
using System.Data.Sql;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.IO;

namespace BugTrackerApplication.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //private ApplicationContext db = new ApplicationContext();
        ApplicationContext context;

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;


        public AccountController()
        {
            context = new ApplicationContext();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        //  POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel login, string returnUrl)
        //public ActionResult Login(LoginViewModel login, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var authManager = HttpContext.GetOwinContext().Authentication;

                AppUser user = userManager.Find(login.Email, login.Password);
                if (user != null)
                {

                    //-- Customer Login --//
                    if (user.UserType == UserType.Customer)
                    {
                        authManager.SignIn();
                        

                        var result = await SignInManager.PasswordSignInAsync(login.Email, login.Password, login.RememberMe, shouldLockout: false);
                        
                        switch (result)
                        {
                            //case SignInStatus.
                            case SignInStatus.Success:
                                return RedirectToLocal(returnUrl);
                            case SignInStatus.LockedOut:
                                return View("Lockout");
                            case SignInStatus.RequiresVerification:
                                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = login.RememberMe });
                            case SignInStatus.Failure:
                            default:
                                ModelState.AddModelError("", "Invalid login attempt.");
                                return View(login);
                        }
                        
                    }


                    //-- Employee Login --//
                    if (user.UserType == UserType.Employee)
                    {
                        authManager.SignIn();
                        

                        var result = await SignInManager.PasswordSignInAsync(login.Email, login.Password, login.RememberMe, shouldLockout: false);
                        
                        switch (result)
                        {
                            //case SignInStatus.
                            case SignInStatus.Success:
                                return RedirectToLocal(returnUrl);
                            case SignInStatus.LockedOut:
                                return View("Lockout");
                            case SignInStatus.RequiresVerification:
                                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = login.RememberMe });
                            case SignInStatus.Failure:
                            default:
                                ModelState.AddModelError("", "Invalid login attempt.");
                                return View(login);
                        }
                        
                    }

                }

            }
            ModelState.AddModelError("", "Invalid Username or Password");
            return View(login);
        }

        

        [AllowAnonymous] 
        public ActionResult DemoLogin(DemoLoginViewModel demoLogin, string returnUrl, string UserName)
        {
            if (ModelState.IsValid)
            {
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var authManager = HttpContext.GetOwinContext().Authentication;
                var users = userManager.Users.ToList();

                var demoUserList = context.Users.Where(u => u.DemoAccount == true).OrderBy(u => u.UserName).ToList();
                ViewBag.DemoUsers = demoUserList;


                var demoUsers = context.AppUsers.Where(u => u.DemoAccount == true).ToList();

                var demoUserViewModel = new List<DemoLoginViewModel>();

                foreach (ApplicationUser user in demoUserList)
                {
                    var thisViewModel = new DemoLoginViewModel();
                    thisViewModel.UserName = user.UserName;
                    thisViewModel.Email = user.Email;
                    thisViewModel.DisplayName = user.DisplayName;
                    demoUserViewModel.Add(thisViewModel);

                }
                return View(demoUserViewModel);
            }

            return View();
        }

        //
        //  GET: Account/DemoLogin...Admin
        [AllowAnonymous]
        public ActionResult DemoLogin_Admin()
        {
            
            return View();
        }

        //
        //  POSTL Account/DemoLogin...Admin
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> DemoLogin_Admin(string UserName, string returnUrl)
        {
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals("DemoAdmin@venellbugtracker.com", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            
            //var user = await UserManager.FindByNameAsync(UserName);
            
            if (user != null)
            {
                await SignInManager.SignInAsync(user, true, false);
                return RedirectToLocal(returnUrl);
            }
            return View();
        }

        //
        //  GET: /Account/DemoLogin...Project Manager
        [AllowAnonymous]
        public ActionResult DemoLogin_ProjectManager()
        {

            return View();
        }

        //
        //  POST: /Account/DemoLogin...Project Manager
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> DemoLogin_ProjectManager(string UserName, string returnUrl)
        {
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals("DemoPM@venellbugtracker.com", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (user != null)
            {
                await SignInManager.SignInAsync(user, true, false);
                return RedirectToLocal(returnUrl);
            }
            return View();
        }

        //
        //  GET: /Account/DemoLogin...Developer
        [AllowAnonymous]
        public ActionResult DemoLogin_Developer()
        {

            return View();
        }

        //
        //  POST: /Account/DemoLogin...Developer
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> DemoLogin_Developer(string UserName, string returnUrl)
        {
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals("DemoDev@venellbugtracker.com", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (user != null)
            {
                await SignInManager.SignInAsync(user, true, false);
                return RedirectToLocal(returnUrl);
            }
            return View();
        }

        
        //
        //  GET: /Account/DemoLogin...Submitter
        [AllowAnonymous]
        public ActionResult DemoLogin_Submitter()
        {
            return View();
        }

        //
        //  POST: /Account/DemoLogin...Submitter
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> DemoLogin_Submitter(string UserName, string returnUrl)
        {
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals("DemoSubmit@venellbugtracker.com", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (user != null)
            {
                await SignInManager.SignInAsync(user, true, false);
                return RedirectToLocal(returnUrl);
            }
            return View();
        }
       

        //
        //  POST: /Account/WelcomeNewCustomer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WelcomeNewCustomer(LoginViewModel login, string id)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var authManager = HttpContext.GetOwinContext().Authentication;

            AppUser user = userManager.Find(login.Email, login.Password);
            if (user != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var userToUpdate = context.Users
                    .Include(i => i.FirstTimeUser)
                    .Where(i => i.Id == id)
                    .Single();

                if (TryUpdateModel(userToUpdate, "",
                    new string[] { "FirstTimeUser" }))
                {
                    try
                    {
                        //userToUpdate.FirstTimeUser = false;
                        user.FirstTimeUser = false;
                        context.SaveChanges();
                    }
                    catch (RetryLimitExceededException /* dex */)
                    {
                        // Log the error (uncomment dex variable and add a line here to write a log.
                        ModelState.AddModelError("", "Unable to update the 'FirstTimeUser' table value to 'False'.");
                    }
                }

                authManager.SignIn();
                return View("Index");
            }

            return View();
        }

        

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig

            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            //ViewBag.Name = new SelectList(context.Roles.ToList(), "Name", "Name");
            //PopulateProfileTypeDropDownList();
            return View();
        }
        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        public async Task<ActionResult> Register(RegisterViewModel model, HttpPostedFileBase Register) //, Developer developer, Submitter submitter)
        {

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, ProfilePicture = model.ProfilePicture, FirstName = model.FirstName, LastName = model.LastName, UserType = model.UserType, Authorized = false, FirstTimeUser = true };
                var result = await UserManager.CreateAsync(user, model.Password); //NOTE: May need to look here to prevent creation or delate momentarily...-EV Redirect to Login 08/02/2021...
                if (user.UserType == UserType.Customer)
                {
                    //-- Assign to Basic Role on Create() --//
                    UserManager.AddToRole(UserManager.FindByName(user.UserName).Id, "Basic");
                    //--------------------------------------//
                    // - Email Admin of new User creation but auto let them in on locked down rights. -EV


                    // Change the Status to Authorized = True by default
                    user.Authorized = true;
                    user.LockoutEnabled = false;

                    if (result.Succeeded)
                    {
                        //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        
                        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                        var callbackUrl = Url.Action("ConfirmEmail", "Account",
                            new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                        
                        

                       
                        TempData["EmailSentToAddress"] = user.Email.ToString();
                        TempData["TcD: FirstName"] = user.FirstName.ToString();
                        TempData["TcD: LastName"] = user.LastName.ToString();

                        

                        return RedirectToAction("CustomerAccessRequest");
                    }
                    AddErrors(result);
                }
                //if (user.UserType == UserType.Employee)
                if (model.UserType == UserType.Employee)
                {
                    //-- Assign to Basic Role on Create() --//

                    
                    UserManager.AddToRole(UserManager.FindByName(user.UserName).Id, "Pending");
                    //--------------------------------------//
                    // - Email Admin of new User pending Approval for EMPLOYEE (Developer, ProjectManager, or Admin status for admintance at Admin;s discretion. -EV
                    user.Authorized = false;
                    user.LockoutEnabled = false;

                    //user.FirstTimeUser = true;
                    // Send an email with this link

                    if (result.Succeeded)
                    {
                        //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                        var callbackUrl = Url.Action("ConfirmEmail", "Account",
                            new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        await UserManager.SendEmailAsync(user.Id,
                            "Confirm your account", "Please confirm your account by clicking <a href=\""
                            + callbackUrl + "\">here</a>");

                        //return RedirectToAction("Index", "Home");
                        TempData["EmailSentToEmployee"] = user.Email;
                        TempData["TeD: FirstName"] = user.FirstName;
                        TempData["TeD: LastName"] = user.LastName;

                        //-- Prevent User from logging in RIGHT AFTER profile created --//

                        
                        return RedirectToAction("EmployeeAccessRequest");
                    }
                    AddErrors(result);

                    
                }


                

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // GET: //Account/CustomerAccessRequest
        
        [AllowAnonymous]
        public ActionResult CustomerAccessRequest(RegisterViewModel model)
        {

            ViewBag.FirstName = TempData["TcD: FirstName"]; //TcD = Temporary Customer Data
            ViewBag.LastName = TempData["TcD: LastName"];
            ViewBag.EmailSentToAddress = TempData["EmailSentToAddress"];

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var authManager = HttpContext.GetOwinContext().Authentication;

            //authManager.SignOut(); //Commenting out after Email registration is complete. 

            return View("CustomerAccessRequest");

        }

        // GET: //Shared/EmployeeAccessRequest
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult EmployeeAccessRequest(RegisterViewModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, UserType = model.UserType, Authorized = false };

            ViewBag.EMPFirstName = TempData["TeD: FirstName"]; //TeD = Temporary Employee Data
            ViewBag.EMPLastName = TempData["TeD: LastName"];
            ViewBag.EmailSentToEmployee = TempData["EmailSentToEmployee"];
            //--- Restrict the Account from Access until allowed/created by administrator ---//
            user.LockoutEnabled = true;
            user.Approved = false;
            user.FirstTimeUser = true;
            //------//
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var authManager = HttpContext.GetOwinContext().Authentication;


            //authManager.SignOut();

            // Refresh the screen...


            return View("EmployeeAccessRequest");
        }


        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
            //----------------------------//
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login","Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}