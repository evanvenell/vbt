using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTrackerApplication.Models
{
    public class Developer : Person
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
        //public virtual RoleAssignment RoleAssignment { get; set; }
    }


    //-- Original Code --//
    //public class Developer : AppUser
    ////public class Developer : AppUser
    //{
    //    public int ID { get; set; }

    //    [Required]
    //    [Display(Name = "Last Name")]
    //    [StringLength(50)]
    //    public string LastName { get; set; }

    //    [Required]
    //    [Column("FirstName")]
    //    [Display(Name = "First Name")]
    //    [StringLength(50)]
    //    public string FirstMidName { get; set; }

    //    //public string Email { get; set; }
    //    public string EmailAddress { get; set; }

    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    //    [Display(Name = "Hire Date")]
    //    public DateTime HireDate { get; set; }

    //    [Display(Name = "Full Name")]
    //    public string FullName
    //    {
    //        get { return FirstMidName + " " + LastName; }
    //    }


    //    public virtual ICollection<Ticket> Tickets { get; set; }
    //    //NOTE:^^^ A Developer can work any number of Tickets, so Tickets is defined as a collection of Ticket entities.

    //    //public virtual RoleAssignment RoleAssignment { get; set; }
    //    //NOTE:^^^ A Developer can only have one Role, so RoleAssignment is defined as a single RoleAssignment entity (which may be null if no Role is assigned).
    //    //public virtual ICollection<AppUser> AppUsers { get; set; }

    //}
}