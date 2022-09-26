using System.ComponentModel.DataAnnotations;

namespace BugTrackerApplication.Models
{
    //public enum Grade
    //{
    //    A, B, C, D, F
    //}

    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public int TicketID { get; set; }
        //NOTE:^^^ An Enrollment record is for a single Ticket, so there's a TicketID foreign key property and a Ticket navigation property.
        public int SubmitterID { get; set; }
        //NOTE:^^^ An Enrollment record is for a single User, so there's a UserID foreign key property and a User navigation Property.

        [DisplayFormat(NullDisplayText = "No grade")] //Added in CDM
        //public Grade? Grade { get; set; }

        public virtual Ticket Ticket { get; set; }
        //NOTE:^^^ An enrollment record is for a single Ticket, so there's a TicketID foreign key property and a Ticket navigation property.
        public virtual Submitter Submitter { get; set; }
        //NOTE:^^^ An Enrollment record is for a single User, so there's a UserID foreign key property and a User navigation Property.
    }
}