using System.Collections.Generic;
using BugTrackerApplication.Models;

namespace BugTrackerApplication.ViewModels
{
    public class DeveloperIndexData
    {
        public IEnumerable<Developer> Developers { get; set; }
        public IEnumerable<Ticket> Tickets { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}