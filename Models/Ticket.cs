using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTrackerApplication.Models
{
    public enum TicketPriority
    {
        Low, Medium, High
    }

    public enum TicketStatus
    {
        New, InProgress, RequestedInfo, Scheduled, WaitingOnUser, Cancelled, Closed, Escalation
    }

    public enum TicketType
    {
        Bugs, Errors, FeatureRequest, OtherComments, TrainingDocumentRequests
    }

    public class Ticket
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Display(Name = "Number")]
        [Display(Name = "Ticket ID")]
        public int TicketID { get; set; }
        public string TicketTitle { get; set; }
        public string TicketDescription { get; set; }

        //[Display(Name = "Developer")]
        //public int AssignedDeveloper { get; set; } 
        //NOTE:^^^ Moving to Project entity because of the relationship between Project and Ticket entities...
        //...each ticket can have a submitter but not an AssignedDeveloper, the PROJECT has the AssignedDeveloper but the ticket details can show...
        //...the assigned develoepr to address the ticket because the ticket is assigned to the project. -EV 01/26/2021...

        [Display(Name = "Submitter")]
        //public int Submitter { get; set; }
        public string Submitter { get; set; }
        //public string ProjectName { get; set; }
        //public int ProjectID { get; set; }
        public TicketPriority? TicketPriority { get; set; }
        public TicketStatus? TicketStatus { get; set; }
        public TicketType? TicketType { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Updated Date")]
        public DateTime Updated { get; set; }

        //[Range(0, 5)]
        //public int Credits { get; set; }

        public int ProjectID { get; set; }
        //NOTE:^^^ A Ticket is assigned to one Project, so there's a ProjectID foreign key and a Project navigation porperty.
        //public int DeveloperID { get; set; }

        //public int? TicketDevelopers { get; set; }


        public virtual Project Project { get; set; }
        //NOTE:^^^ A Ticket is assigned to one Project, so there's a ProjectID foreign key and a Project navigation porperty.
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        //NOTE:^^^ A Ticket can have any number of Users enrolled in it, so the Enrollments navigation property is a collection.
        public virtual ICollection<Developer> Developers { get; set; }
        //NOTE:^^^ A Ticket may be worked by multiple Developers, so the Deveopers navigation property is a collection.


        public virtual ICollection<Comment> Comments { get; set; }
        //NOTE:^^^ A Ticket can have multiple comment's, so Comment is defined as a collection in the Ticket entity

        public virtual ICollection<TicketAttachments> TicketAttachments { get; set; }
    }
}