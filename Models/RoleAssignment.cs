using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTrackerApplication.Models
{
    public enum RoleName
    {
        Admin, ProjectManager, Developer
    }
    //, Submitter //Submitter will probably be ONLY the Users-EV 01/22/2021...

    public class RoleAssignment
    {
        //[Key]
        //[ForeignKey("Developer")]
        //NOTE:^^^ When the is a One-to-Zero-or-One Relationship or a One-to-One Relationship between two entites (such as between RoleAssignment and Developer), EF Can't work...
        //... out which end of the relaionship is the principal and which end is dependent. One-to-One Relationships have a referrence navigation Property in each class to the other class.

        //public int DeveloperID { get; set; }
        public int RoleAssignmentID { get; set; }

        //[StringLength(50)]
        [Display(Name = "Role")]
        //public string RoleName { get; set; }
        public RoleName? RoleName { get; set; }

        public string Role
        {
            get { return RoleName.ToString(); }
        }

        //public virtual Developer Developer { get; set; }
        //NOTE:^^^ The Developer entity has a nullable RoleAssignment navigation property (because a Developer might not have a role assignment), and the RoleAssignment entity...
        //... has a non-nullable Developer navigation property.
    }
}