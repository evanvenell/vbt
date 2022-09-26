using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity.EntityFramework;
using BugTrackerApplication.Models;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BugTrackerApplication.DAL;
using System.Data;
using System.Data.Entity;
using System.Net;
using BugTrackerApplication.ViewModels;
using System.Data.Entity.Infrastructure;
using System.Data.Sql;
using System.Data.SqlClient;

namespace BugTrackerApplication.ViewModels
{
    public class RoleManagerViewModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string UserName { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }


        public string Email { get; set; }
        //public IEnumerable<IdentityRole> Roles { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<AppRole> AppRoles { get; set; }
        //public IEnumerable<AppUser> AppUsers { get; set; }
        //public IEnumerable<IdentityRole> IdentityRoles { get; set; }
        public IEnumerable<Microsoft.AspNet.Identity.EntityFramework.IdentityRole> IdentityRoles { get; set; }
    }

    //Manage User Roles
    public class ManageUserRolesViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool Selected { get; set; }
    }
}