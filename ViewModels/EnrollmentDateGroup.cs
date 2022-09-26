using System;
using System.ComponentModel.DataAnnotations;

namespace BugTrackerApplication.ViewModels
{
    public class EnrollmentDateGroup
    {
        [DataType(DataType.Date)]
        public DateTime? EnrollmentDate { get; set; }

        public int SubmitterCount { get; set; }
    }
}