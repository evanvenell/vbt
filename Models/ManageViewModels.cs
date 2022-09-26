using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Web;
//using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using BugTrackerApplication.Models;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using BugTrackerApplication.DAL;
using System.Data;
using System.Data.Entity;
using System.Net;
using BugTrackerApplication.ViewModels;
using System.Data.Entity.Infrastructure;
using System.Data.Sql;
using System.Data.SqlClient;

namespace BugTrackerApplication.Models
{
    public class IndexViewModel
    {
        public string FirstName { get; set; }
        public string DevFirstName {get; set;}
        public string SubFirstName { get; set; }
        public string LastName { get; set; }
        public string DevLastName { get; set; }
        public string SubLastName { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        public string Email { get; set; }
        public string DevEmail { get; set; }
        public string SubEmail { get; set; }
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
        // New User Code//

        public string JobTitle { get; set; }

        public byte[] ProfilePicture { get; set; }

        [Display(Name = "Role")]
        public string AssignedRole { get; set; }
        
        //-- Statistics --//
        [Display(Name = "Tickets")]
        public int AssociatedTickets { get; set; }

        [Display(Name = "Projects")]
        public int AssociatedProjects { get; set; }

        [Display(Name = "Comments")]
        public int AssociatedComments { get; set; }
        //-- ./Statistics --//
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<Ticket> Tickets { get; set; }
        public IEnumerable<Project> Projects { get; set; }
        public IEnumerable<AppRole> AppRoles { get; set; }
        public IEnumerable<AppUser> AppUsers { get; set; }
        public IEnumerable<Developer> Developers { get; set; }
        public IEnumerable<Submitter> Submitters { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
        //public bool FirstTimeUser { get; set; }

        // About Me Fields (Optional)
        public string EducationOrExperience { get; set; }
        public string Location { get; set; }
        public string Skills { get; set; }
        public string Notes { get; set; }
        //--------------------------//
        
    }



    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }

    //-- Custom Code --//
    
    public class ProfileSettingsViewModel
    {
        public string FirstMidName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return FirstMidName + " " + LastName;
            }
        }

        public string EmailAddress { get; set; }
        public int UserType { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }

        public byte[] ProfilePicture { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<AppRole> AppRoles { get; set; }
        public IEnumerable<Microsoft.AspNet.Identity.EntityFramework.IdentityRole> IdentityRoles { get; set; }
        public IEnumerable<Developer> Developers { get; set; }
        //public IEnumerable<Submitter> Submitters { get; set; }
        public IEnumerable<Person> People { get; set; }
    }

    public class SubmitterSettingsViewModel
    {
        public string FirstMidName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public DateTime EnrollmentDate { get; set; }

        public byte[] ProfilePicture { get; set; }

        public IEnumerable<Person> People { get; set; }
    }
    //----------------//

    //-- Timeline --//
    //public enum TimelinePostType
    //{
    //    Email, FriendRequest, Comment, Photo, Video
    //}
    //public class AccountTimeLineViewModel
    //{
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public string Email { get; set; }
    //    public DateTime StartDate { get; set; }
    //    public DateTime PostDate { get; set; }

    //    public DateTime PostAge { get; set; }

    //    public string Author { get; set; }
        
    //    public string PostBody { get; set; }
    //    public TimelinePostType TimelinePostType { get; set; }
    //    public string ConditionalPostHeader { get; set; }
    //    public string ConditionalPostLink { get; set; } //NOTE: Similar to returnUrl. -EV 09/03/2021...

    //    public byte[] PostAttachment { get; set; }



    //}
    //---------------//
}