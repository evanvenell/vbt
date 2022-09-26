using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTrackerApplication.Models
{
    public enum AttachmentType
    {
        Image, Email //NOTE: Will add more later IF I build out email integration further with PHP. -EV 09/17/2021...
    }
    public class TicketAttachments
    {
        public int ID { get; set; }

        public int TicketID { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public string Author { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Uploaded")]
        public DateTime Uploaded { get; set; }

        public byte[] File { get; set; }

        [Display(Name = "Type")]
        public AttachmentType AttachmentType { get; set; }

        //  Nav Properties
        public virtual Ticket Ticket { get; set; }
        
    }
}