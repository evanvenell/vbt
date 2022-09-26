using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    //public enum ProfileType
    //{
        //Developer, Submitter
    //    Customer, Employee
    //}
    public abstract class Person
    //public abstract class Person : AppProfile
    //public abstract class Person
    {
        public int ID { get; set; }


        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Column("FirstName")]
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }

        //public string Email { get; set;}
        public string EmailAddress { get; set; }

        //-- Identity --//
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public string AppUserID { get; set; }

        //public ProfileType? ProfileType { get; set; }
        public UserType? UserType { get; set; }

        public string UserId { get; set; }
        //--------------------------------------------//

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return FirstMidName + " " + LastName;
            }
        }
        public ICollection<AppUser> AppUsers { get; set; } // NOTE: WORKED PARTIALLY. 
        
        //public AppUser IdentityUser { get; set; }
        //public virtual AppUser User { get; set; }
        //public string PersonProperty { get; set; }
        //public ProfileType? ProfileType { get; set; }
    }
}