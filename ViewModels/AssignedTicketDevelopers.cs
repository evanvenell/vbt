using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerApplication.ViewModels
{
    public class AssignedTicketDevelopers
    {
        //public int TicketID { get; set; }
        //public string Title { get; set; }
        //public bool Assigned { get; set; }
        public int DeveloperID { get; set; }
        public string FirstMidName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return FirstMidName + " " + LastName;
            }
        }

        public bool Assigned { get; set; }
    }
}