using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using BugTrackerApplication.DAL;

namespace BugTrackerApplication.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "EmailAddress")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "EmailAddress")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "EmailAddress")]
        [EmailAddress]
        public string Email { get; set; }

        public string ReturnUrl { get; set; }

        public bool FirstTimeUser { get; set; }
        public bool Approved { get; set; } //NOTE: Should only be CUSTOMERS not EMPLOYEES. -EV 

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    //------//
    //public enum ProfileType
    //public enum UserType
    //{
    //    Submitter, Developer
    //}
    //------//
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "EmailAddress")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        //-- Custom Controls for Existing DB's --//
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public byte[] ProfilePicture { get; set; }
        
        //[Required]
        //public ProfileType? ProfileType { get; set; }
        public UserType? UserType { get; set; }


        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        //---------------------------------------//
    }

    //public class ThankYouForRegisteringViewModel
    public class CustomerAccessRequestViewModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }

    

    public class EmployeeAccessRequestViewModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "EmailAddress")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "EmailAddress")]
        public string Email { get; set; }
    }

    public class DemoLoginViewModel
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string DisplayName { get; set; }

        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
 
        public IEnumerable<AppUser> AppUsers { get; set; }
        public IEnumerable<Developer> Developers { get; set; }
        public IEnumerable<Submitter> Submitters { get; set; }
    }

    //public class AdminPanelIndexView
    //{
    //    public string Id { get; set; }
    //    public string FirstName { get; set; }
    //    public string Lastname { get; set; }
    //    public string Email { get; set; }


    //}
}
