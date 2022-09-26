using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace BugTrackerApplication.Models
{
    public enum Role
    {
        //  Main Roles:
        Admin, Developer, ProjectManager, Basic, Pending,
        //  Sub/Temporary Roles:
        DevSelfModify, SubSelfModify,
        //  Demo_Roles:
        Demo_Admin, Demo_Developer, Demo_ProjectManager, Demo_Submitter //NOTE: Using "Demo_Submitter" instead of "Basic" to further differentiate it from normal roles. -EV 
    }
    public class AppRole : IdentityRole
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }

        public Role Role { get; set; }
    }
}