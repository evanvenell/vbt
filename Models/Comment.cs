using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTrackerApplication.Models
{
    public class Comment
    {
        //[Key]
        //[ForeignKey("Ticket")]
        public int ID { get; set; }

        //[Column("Ticket")]
        //[Display(Name = "Ticket ID")]
        public int TicketID { get; set; }
        //public int DeveloperID { get; set; }
        //public int UserID { get; set; }

        [Display(Name = "Title")]
        public string CommentTitle { get; set; }

        //public int Commenter { get; set; }
        public string Commenter { get; set; }

        public string Message { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        //public ICollection<Ticket> Tickets { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}