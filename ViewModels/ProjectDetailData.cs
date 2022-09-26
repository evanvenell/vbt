using System.Collections.Generic;
using BugTrackerApplication.Models;

namespace BugTrackerApplication.ViewModels
{
    public class ProjectDetailData
    {
        //public IEnumerable<Developer> Developers { get; set; }
        //public IEnumerable<Project> Projects { get; set; }
        public IEnumerable<Developer> Developers { get; set; }
        public IEnumerable<Ticket> Tickets { get; set; }

        public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}