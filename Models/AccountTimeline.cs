using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using BugTrackerApplication.Models;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using BugTrackerApplication.DAL;
using System.Data;
using System.Data.Entity;
using System.Net;
using BugTrackerApplication.ViewModels;
using System.Data.Entity.Infrastructure;
using System.Data.Sql;
using System.Data.SqlClient;
namespace BugTrackerApplication.Models
{
    public enum TimelinePostType
    {
        Email, FriendRequest, Comment, Photo, Video
    }
    public class AccountTimeline
    {
        //-- AccountTimeLine Posts --//
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public int DeveloperId { get; set; }
        public int SubmitterId { get; set; }
        public int TicketId { get; set; }
        public int ProjectId { get; set; }
        public int CommentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime PostDate { get; set; }

        public DateTime PostAge { get; set; }

        public string Author { get; set; }

        public string PostBody { get; set; }
        public TimelinePostType TimelinePostType { get; set; }
        public string ConditionalPostHeader { get; set; }
        public string ConditionalPostLink { get; set; } //NOTE: Similar to returnUrl. -EV 09/03/2021...

        public byte[] PostAttachment { get; set; }
        //---------------------------//
        public ICollection<Developer> Developers { get; set; }
        public ICollection<Submitter> Submitters { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<Project> Projects { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<AppUser> AppUsers { get; set; }
    }
}