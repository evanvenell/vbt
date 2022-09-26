using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace BugTrackerApplication.Models
{
    public enum UserType
    {
        //Submitter, Developer
        Customer, Employee
        //Submitter, Developer, ProjectManager, Admin
    }
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public UserType? UserType { get; set; }

        // First Time Users (Employees) //
        //[Required]
        public bool Approved { get; set; } //NOTE: IF APPROVED = true, THEN Access = Enabled
                                            //... IF APPROVED = false, THEN Access = Disabled and redirect to Lockout for First TIme. 
        //[Required]
        public bool FirstTimeUser { get; set; }

        //--- Custom User Properties for Profile page (all optional) ---//
        [Display(Name = "Developer ID")]
        public int DeveloperID { get; set; }

        [Display(Name = "Submitter ID")]
        public int SubmitterID { get; set; }

        public string DisplayName { get; set; }
        public string EducationOrExperience { get; set; }
        public string Location { get; set; }
        public string Skills { get; set; }
        public string Notes { get; set; }


        //public string PhoneNumber { get; set; } //Already declared in identity.
        public bool Employee { get; set; } //If set to FALSE then it just displays customer under their profile. 

        public bool DemoAccount { get; set; } //IF False then it is a normal account, IF True, THEN it is a Demo_### account. 
        //public string Position { get; set; }
        public string JobTitle { get; set; }

        //--------------------------------------------------------------//
        public bool Authorized { get; set; }

        [Display(Name = "Profile Picture")]
        public byte[] ProfilePicture { get; set; }

        public ICollection<Person> People { get; set; }
        //public virtual Person Person { get; set; }
        //public Person Person { get; set; }
    }

    //public class AppUser : ApplicationUser
    //public class AppUser : IdentityUser
    //public class AppUser
    //{
    //    //override public string Id { get; set; }
    //    public string AppUserID { get; set; }

    //    //public string PersonID { get; set; }
    //    public int PersonID { get; set; }
    //    //override public string UserName { get; set; }

    //    //public bool DemoUser { get; set; }

    //    //public virtual ICollection<Person> People { get; set; } //NOTE: Partially worked. -EV
    //}

    //public class AppProfile
    //{
    //    public string ID { get; set; }
    //    public string ProfileName { get; set; }
    //    public ICollection<Person> People { get; set; }
        
    //}
}