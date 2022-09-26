using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTrackerApplication.Models
{
    public class Project
    {
        public int ProjectID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string ProjectName { get; set; }

        [Display(Name = "Description")]
        public string ProjectDescription { get; set; }

        [Display(Name = "Assigned Developer")]
        public string AssignedDeveloper { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Administrator")]
        public int? DeveloperID { get; set; }
        //NOTE^^^ A Project may or may not have an administrator, and an administrator is always a Developer. Therefore DeveloperID property is included as the foreign key...
        //... to the Developer entity.

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual Developer Administrator { get; set; }
        //NOTE^^^ A Project may or may not have an administrator, and an administrator is always a Developer. Therefore DeveloperID property is included as the foreign key...
        //... to the Developer entity.

        //public virtual ICollection<Developer> Developers { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        //NOTE:^^^ A Project may have many Tickets, so there's a Tickets navigation property.
    }
}