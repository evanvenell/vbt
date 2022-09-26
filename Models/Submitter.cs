using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTrackerApplication.Models
{
    public class Submitter : Person
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }


    //-- Original Code --//
    //public class Submitter
    //public class Submitter : AppUser
    //{
    //    public int ID { get; set; }

    //    [StringLength(50)]
    //    public string LastName { get; set; }

    //    [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
    //    [Column("FirstName")]
    //    public string FirstMidName { get; set; }

    //    //public string Email { get; set; }
    //    public string EmailAddress { get; set; }

    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    //    public DateTime EnrollmentDate { get; set; }

    //    [Display(Name = "Full Name")]
    //    public string FullName
    //    {
    //        get { return FirstMidName + " " + LastName; }
    //    }


    //    public virtual ICollection<Enrollment> Enrollments { get; set; }

    //    //public virtual ICollection<AppUser> AppUsers { get; set; }
    //}
}