namespace BugTrackerApplication.Migrations
{
    using BugTrackerApplication.Models;
    using BugTrackerApplication.DAL;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Data;
    using System.Net;
    using BugTrackerApplication.ViewModels;
    using System.Data.Entity.Infrastructure;
    using System.Data.Sql;
    using System.Data.SqlClient;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Cookies;
    using Owin;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Globalization;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;
    using BugTrackerApplication.Filters;
    using BugTrackerApplication.CustomFilters;

    //internal sealed class Configuration : DbMigrationsConfiguration<BugTrackerApplication.DAL.ApplicationContext>
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        

        //protected override void Seed(BugTrackerApplication.DAL.ApplicationContext context)
        //protected override void Seed(ApplicationContext context)
        protected override void Seed(ApplicationContext context)
        {
            //-- submitters --//
            var submitters = new List<Submitter>
            {
                //new Submitter { FirstMidName = "Carson",   LastName = "Alexander", EmailAddress = "calexander@gmail.com",
                //    EnrollmentDate = DateTime.Parse("2021-09-01"), UserType = UserType.Customer}, //ProfileType = ProfileType.Submitter },
                //new Submitter { FirstMidName = "Meredith", LastName = "Alonso", EmailAddress = "malonzo@yahoo.com",
                //    EnrollmentDate = DateTime.Parse("2021-09-01"), UserType = UserType.Customer }, //ProfileType = ProfileType.Submitter },
                //new Submitter { FirstMidName = "Arturo",   LastName = "Anand", EmailAddress = "aanand@sbcglobal.net",
                //    EnrollmentDate = DateTime.Parse("2015-09-01"), UserType = UserType.Customer}, //ProfileType = ProfileType.Submitter },
                //new Submitter { FirstMidName = "Gytis",    LastName = "Barzdukas", EmailAddress = "gbarzdukas@outlook.com",
                //    EnrollmentDate = DateTime.Parse("2021-09-01"), UserType = UserType.Customer}, //ProfileType = ProfileType.Submitter },
                //new Submitter { FirstMidName = "Yan",      LastName = "Li", EmailAddress = "yli@gmail.com",
                //    EnrollmentDate = DateTime.Parse("2021-09-01"), UserType = UserType.Customer}, //ProfileType = ProfileType.Submitter },
                //new Submitter { FirstMidName = "Peggy",    LastName = "Justice", EmailAddress = "pjustice@outlook.com",
                //    EnrollmentDate = DateTime.Parse("2015-09-01"), UserType = UserType.Customer }, //ProfileType = ProfileType.Submitter },
                //new Submitter { FirstMidName = "Laura",    LastName = "Norman", EmailAddress = "lnorman@gmail.com",
                //    EnrollmentDate = DateTime.Parse("2014-09-01"),  UserType = UserType.Customer}, //ProfileType = ProfileType.Submitter },
                //new Submitter { FirstMidName = "Nino",     LastName = "Olivetto", EmailAddress = "nolivetto@sbcglobal.net",
                //    EnrollmentDate = DateTime.Parse("2010-08-11"),  UserType = UserType.Customer}, //ProfileType = ProfileType.Submitter },
                //-- Demo - Authenticated BUILT IN Submitter(s) --//
                new Submitter { FirstMidName = "DemoS", LastName = "Submit", EmailAddress = "DemoSubmit@venellbugtracker.com",
                    EnrollmentDate = DateTime.Parse("2021-04-12"),  UserType = UserType.Customer, UserId = "0bc92304-22cd-4ff4-979d-71e2d6c126a0",}, //ProfileType = ProfileType.Submitter },
                //-------------------------------------------//
            };

            submitters.ForEach(s => context.Submitters.AddOrUpdate(p => p.LastName, s));
            context.SaveChanges();
            //------//


            //-- developers --//
            var developers = new List<Developer>
            {
                //new Developer { FirstMidName = "Evan", LastName = "Venell", EmailAddress = "evanVenell@mailinator.com",
                //    HireDate = DateTime.Parse("2021-01-22"), UserType = UserType.Employee}, //ProfileType = ProfileType.Developer },
                //new Developer { FirstMidName = "Bobby", LastName = "Davis", EmailAddress = "bdavis@core-techs.net",
                //    HireDate = DateTime.Parse("2021-01-22"), UserType = UserType.Employee }, //ProfileType = ProfileType.Developer },
                //new Developer { FirstMidName = "Daniel", LastName = "Dean", EmailAddress = "developerDD@mailinator.com",
                //    HireDate = DateTime.Parse("2021-01-22"), UserType = UserType.Employee }, //ProfileType = ProfileType.Developer },
                //new Developer { FirstMidName = "Daniel", LastName = "Hebbard", EmailAddress = "arcadia_sin@mailinator.com",
                //    HireDate = DateTime.Parse("2021-01-22"), UserType = UserType.Employee }, //ProfileType = ProfileType.Developer },
                //new Developer { FirstMidName = "Jason", LastName = "Twichell", EmailAddress = "projectmanagerJT@mailinator.com",
                //    HireDate = DateTime.Parse("2021-01-22"), UserType = UserType.Employee }, //ProfileType = ProfileType.Developer },
                //-- Demo - Authenticated BUILT IN Submitter(s) --//
                new Developer { FirstMidName = "DemoA", LastName = "Admin", EmailAddress = "DemoAdmin@venellbugtracker.com",
                    HireDate = DateTime.Parse("2021-04-12"), UserType = UserType.Employee, UserId = "8a750bf1-d4b2-4a49-bd10-0ffd7a704de4", }, //ProfileType = ProfileType.Developer },
                new Developer { FirstMidName = "DemoD", LastName = "Dev", EmailAddress = "DemoDev@venellbugtracker.com",
                    HireDate = DateTime.Parse("2021-04-12"), UserType = UserType.Employee, UserId = "31cbea4c-40c6-4f2a-ab74-b2b15bf59f38", }, //ProfileType = ProfileType.Developer },
                new Developer { FirstMidName = "DemoP", LastName = "PM", EmailAddress = "DemoPM@venellbugtracker.com",
                    HireDate = DateTime.Parse("2021-04-12"), UserType = UserType.Employee, UserId = "57fb18a0-415b-48fb-a634-ade1d9e7d952", }, //ProfileType = ProfileType.Developer },
                //-------------------------------------------//
            };

            developers.ForEach(s => context.Developers.AddOrUpdate(p => p.LastName, s));
            context.SaveChanges();
            //------//


            //-- projects --//
            var projects = new List<Project>
            {
                new Project { ProjectName = "Demo Project 1", ProjectDescription = "This is project number 1.", Budget = 350000,
                    AssignedDeveloper = developers.Single(d => d.LastName == "PM").FullName,
                    StartDate = DateTime.Parse("2021-01-22"),
                    DeveloperID = developers.Single(i => i.LastName == "Dev").ID },
                new Project { ProjectName = "Demo Project 2", ProjectDescription = "Testing PM self assignment on create.", Budget = 100000,
                    AssignedDeveloper = developers.Single(d => d.LastName == "PM").FullName,
                    StartDate = DateTime.Parse("2021-01-22"),
                    DeveloperID = developers.Single(i => i.LastName == "Dev").ID },
                new Project { ProjectName = "DH_Blog", ProjectDescription = "My blog project.", Budget = 350000,
                    AssignedDeveloper = developers.Single(d => d.LastName == "Dev").FullName,
                    StartDate = DateTime.Parse("2021-01-22"),
                    DeveloperID = developers.Single(i => i.LastName == "Dev").ID },
                new Project { ProjectName = "DH_BugTracker", ProjectDescription = "My BugTracker project.", Budget = 100000,
                    AssignedDeveloper = developers.Single(d => d.LastName == "Admin").FullName,
                    StartDate = DateTime.Parse("2021-01-22"),
                    DeveloperID = developers.Single(i => i.LastName == "Admin").ID },
            };

            projects.ForEach(s => context.Projects.AddOrUpdate(p => p.ProjectName, s));
            context.SaveChanges();
            //------//


            //-- tickets --//
            var tickets = new List<Ticket>
            {
                new Ticket {TicketID = 1050, TicketTitle = "Great work", //Credits = 3,
                    TicketDescription = "Keep plugging in code, you're getting there.",
                    TicketType = TicketType.OtherComments,
                    TicketStatus = TicketStatus.Closed,
                    TicketPriority = TicketPriority.High,
                    ProjectID = projects.Single(s => s.ProjectName == "Demo Project 1").ProjectID,
                    //AssignedDeveloper = developers.Single(d => d.LastName == "Venell").ID,
                    Submitter = submitters.Single(u => u.LastName == "Submit").FullName,
                    Created=DateTime.Parse("2021-01-22"),
                    Updated=DateTime.Parse("2021-01-22"),
                    //TicketComments = new List<TicketComment>(),
                    Comments = new List<Comment>(),
                    Developers = new List<Developer>()
                },
                new Ticket {TicketID = 4022, TicketTitle = "Release bug: Incorrect Date/Time", //Credits = 3,
                    TicketDescription = "Recent release of the program has the incorrect system date and time. Needs to reflect 2021 new year.",
                    TicketType = TicketType.Bugs,
                    TicketStatus = TicketStatus.Scheduled,
                    TicketPriority = TicketPriority.High,
                    ProjectID = projects.Single(s => s.ProjectName == "Demo Project 2").ProjectID,
                    //AssignedDeveloper = developers.Single(d => d.LastName == "Davis").ID,
                    Submitter = submitters.Single(u => u.LastName == "Submit").FullName,
                    Created=DateTime.Parse("2021-01-22"),
                    Updated=DateTime.Parse("2021-01-22"),
                    //TicketComments = new List<TicketComment>(),
                    Comments = new List<Comment>(),
                    Developers = new List<Developer>()
                },
                new Ticket {TicketID = 4041, TicketTitle = "Aesthetics please", //Credits = 3,
                    TicketDescription = "Improve the overall UI to increase UX feedback from submitters (submitters).",
                    TicketType = TicketType.FeatureRequest,
                    TicketStatus = TicketStatus.New,
                    TicketPriority = TicketPriority.Low,
                    ProjectID = projects.Single(s => s.ProjectName == "DH_BugTracker").ProjectID,
                    //AssignedDeveloper = developers.Single(d => d.LastName == "Twichell").ID,
                    Submitter = submitters.Single(u => u.LastName == "Submit").FullName,
                    Created=DateTime.Parse("2021-01-22"),
                    Updated=DateTime.Parse("2021-01-22"),
                    //TicketComments = new List<TicketComment>(),
                    Comments = new List<Comment>(),
                    Developers = new List<Developer>()
                },
                new Ticket {TicketID = 1045, TicketTitle = "What's new Scoobydoo release 1.1.12", //Credits = 4,
                    TicketDescription = "Requesting a change notes for the latest version released by vendor.",
                    TicketType = TicketType.TrainingDocumentRequests,
                    TicketStatus = TicketStatus.RequestedInfo,
                    TicketPriority = TicketPriority.Low,
                    ProjectID = projects.Single(s => s.ProjectName == "Demo Project 2").ProjectID,
                    //AssignedDeveloper = developers.Single(d => d.LastName == "Venell").ID,
                    Submitter = submitters.Single(u => u.LastName == "Submit").FullName,
                    Created=DateTime.Parse("2021-01-22"),
                    Updated=DateTime.Parse("2021-01-22"),
                    //TicketComments = new List<TicketComment>(),
                    Comments = new List<Comment>(),
                    Developers = new List<Developer>()
                },
                new Ticket {TicketID = 3141, TicketTitle = "Migration Failure - P1", //Credits = 4,
                    TicketDescription = "The recent Complex Data Model migration failed due to APp layout and inconsistent navigation properties between entities.",
                    TicketType = TicketType.Errors,
                    TicketStatus = TicketStatus.WaitingOnUser,
                    TicketPriority = TicketPriority.High,
                    ProjectID = projects.Single(s => s.ProjectName == "Demo Project 1").ProjectID,
                    //AssignedDeveloper = developers.Single(d => d.LastName == "Twichell").ID,
                    Submitter = submitters.Single(u => u.LastName == "Submit").FullName,
                    Created=DateTime.Parse("2021-01-22"),
                    Updated=DateTime.Parse("2021-01-22"),
                    //TicketComments = new List<TicketComment>(),
                    Comments = new List<Comment>(),
                    Developers = new List<Developer>()
                },
                new Ticket {TicketID = 2021, TicketTitle = "EmailAddress field limit too short", //Credits = 3,
                    TicketDescription = "Submitters have reported that longer company names do not fit in field limits that max out at 50 characters.",
                    TicketType = TicketType.Bugs,
                    TicketStatus = TicketStatus.InProgress,
                    TicketPriority = TicketPriority.Medium,
                    ProjectID = projects.Single(s => s.ProjectName == "DH_Blog").ProjectID,
                    //AssignedDeveloper = developers.Single(d => d.LastName == "Dean").ID,
                    Submitter = submitters.Single(u => u.LastName == "Submit").FullName,
                    Created=DateTime.Parse("2021-01-22"),
                    Updated=DateTime.Parse("2021-01-22"),
                    //TicketComments = new List<TicketComment>(),
                    Comments = new List<Comment>(),
                    Developers = new List<Developer>()
                },
                new Ticket {TicketID = 2042, TicketTitle = "Still cannot access the SQL Database", //Credits = 4,
                    TicketDescription = "The provided credentials did not work to access the SL Database, requesting new ones.",
                    TicketType = TicketType.Errors,
                    TicketStatus = TicketStatus.Escalation,
                    TicketPriority = TicketPriority.High,
                    ProjectID = projects.Single(s => s.ProjectName == "DH_BugTracker").ProjectID,
                    //AssignedDeveloper = developers.Single(d => d.LastName == "Davis").ID,
                    Submitter = submitters.Single(u => u.LastName == "Submit").FullName,
                    Created=DateTime.Parse("2021-01-22"),
                    Updated=DateTime.Parse("2021-01-22"),
                    //TicketComments = new List<TicketComment>(), //NOTE: May need a second piece to tie these in together since each ticketComment is assigned to only one ticket but one ticket can have multiple ticketComment's. -EV 02/18/2021...
                    Comments = new List<Comment>(),
                    Developers = new List<Developer>()
                },
            };

            //tickets.ForEach(s => context.Tickets.AddOrUpdate(p => p.TicketTitle, s));
            tickets.ForEach(s => context.Tickets.AddOrUpdate(p => p.TicketID, s));
            context.SaveChanges();
            //------//

            //-- comments --//
            var comments = new List<Comment>
            {
                new Comment {
                    TicketID = tickets.Single(t => t.TicketID == 1050).TicketID,
                    CommentTitle = "Add some more gray in there",
                    Commenter = developers.Single(i => i.LastName == "PM").FullName,
                    Message = "I want to see more use of gray in the dashboard.",
                    Created = DateTime.Parse("2021-03-02"),
                },
                new Comment {
                    TicketID = tickets.Single(t => t.TicketID == 1050).TicketID,
                    CommentTitle = "Good job with the use of gray",
                    Commenter = developers.Single(i => i.LastName == "PM").FullName,
                    Message = "Nice work on adding that gray. Now work on making the side navbar collapsible through toggling with a button in the UI.",
                    Created = DateTime.Parse("2021-03-02"),
                },
                new Comment {
                    TicketID = tickets.Single(t => t.TicketID == 4022).TicketID,
                    CommentTitle = "Timezone not set correctly",
                    Commenter = developers.Single(i => i.LastName == "Dev").FullName,
                    Message = "The timezone is set to Pacific Standard time and should be Central Standard time instead.",
                    Created = DateTime.Parse("2021-03-02"),
                },
                new Comment {
                    TicketID = tickets.Single(t => t.TicketID == 4041).TicketID,
                    CommentTitle = "Border radius change to 15px;",
                    Commenter = developers.Single(i => i.LastName == "Dev").FullName,
                    Message = "Adjust the corners of the container box's to make them more rounded so they pop better for the end submitter.",
                    Created = DateTime.Parse("2021-03-02"),
                },
                new Comment {
                    TicketID = tickets.Single(t => t.TicketID == 1045).TicketID,
                    CommentTitle = "Assembling document for submission",
                    Commenter = developers.Single(i => i.LastName == "PM").FullName,
                    Message = "Researching documentation and assembling documentation from templates. Should have something before the EoW this week.",
                    Created = DateTime.Parse("2021-03-02"),
                },
                new Comment {
                    TicketID = tickets.Single(t => t.TicketID == 3141).TicketID,
                    CommentTitle = "JoinTable failed the most recent databse update",
                    Commenter = developers.Single(i => i.LastName == "PM").FullName,
                    Message = "The JoinTable for TicketDeveloper has failed it's latest seed data databasde migration update, please rollback to version '202102092135319_FullNamePropAddedToSubmitterEntity'.",
                    Created = DateTime.Parse("2021-03-02"),
                },
                new Comment {
                    TicketID = tickets.Single(t => t.TicketID == 2021).TicketID,
                    CommentTitle = "EmailAddress field limit increase minimum",
                    Commenter = developers.Single(i => i.LastName == "PM").FullName,
                    Message = "Please further increase the field limit size to AT LEAST 150 characters and ensure regexp's work as expected to detect the '@' symbol and if not display an error state requiring it.",
                    Created = DateTime.Parse("2021-03-02"),
                },
                new Comment {
                    TicketID = tickets.Single(t => t.TicketID == 2042).TicketID,
                    CommentTitle = "SQL Access Credentials",
                    Commenter = developers.Single(i => i.LastName == "Admin").FullName,
                    Message = "Try the following credentials: Submittername: LeeeroyJenkinsss | Password: wh3reTh3WhiteWom3n@t?   | Please let me know if this does not work. It should be noted that they really like the film 'Blazing Saddles' here.",
                    Created = DateTime.Parse("2021-03-02"),
                },
            };

            comments.ForEach(s => context.Comments.AddOrUpdate(p => p.ID, s)); //<<<----THIS ONE. -EV 04/26/2021...
            //comments.ForEach(s => context.Comments.AddOrUpdate(p => p.TicketID, s));
            context.SaveChanges();
            //------//


            


            //-- Seed Demo User Roles --//



            //------//


            //-- appRoles --//
            var appRoles = new List<IdentityRole>
            {
                //  Main Roles:
                new IdentityRole { Name = "Admin" },
                new IdentityRole { Name = "Developer" },
                new IdentityRole { Name = "Project Manager" },
                new IdentityRole { Name = "Basic" },
                new IdentityRole { Name = "Pending" },
                //  Sub Roles:
                new IdentityRole { Name = "DevSelfModify" },
                new IdentityRole { Name = "SubSelfModify" },
                //  Demo_Roles:
                new IdentityRole { Name = "Demo_Admin" },
                new IdentityRole { Name = "Demo_Developer" },
                new IdentityRole { Name = "Demo_ProjectManager" },
                new IdentityRole { Name = "Demo_Submitter" },
            };

            appRoles.ForEach(s => context.Roles.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();
            //------//

            //-- appUsers - superadmin --//
            if (!context.Users.Any(u => u.UserName == "superadmin@venellbugtracker.com"))
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var userToInsert = new ApplicationUser
                {
                    UserName = "superadmin@venellbugtracker.com",
                    FirstName = "Super",
                    LastName = "Admin",
                    PhoneNumber = "8325856877",
                    PhoneNumberConfirmed = true,
                    Email = "superadmin@venellbugtracker.com",
                    EmailConfirmed = true,
                    Notes = "Built in Admin account for emergency use only. Also built in in case the Authenticated user admins loose their account roles.",
                    LockoutEnabled = false,
                    Authorized = true,
                    Approved = true,
                    UserType = UserType.Employee,
                    DemoAccount = false,
                };
                userManager.Create(userToInsert, "Isit@Secret1?");
                userManager.AddToRole(userToInsert.Id, "Admin");
            }

            if (!context.Users.Any(u => u.UserName == "DemoAdmin@venellbugtracker.com"))
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var userToInsert = new ApplicationUser
                {
                    UserName = "DemoAdmin@venellbugtracker.com",
                    DeveloperID = 2,
                    FirstName = "DemoA",
                    LastName = "Admin",
                    PhoneNumber = "8325856877",
                    PhoneNumberConfirmed = true,
                    Email = "DemoAdmin@venellbugtracker.com",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    Authorized = true,
                    Approved = true,
                    UserType = UserType.Employee,
                    DemoAccount = true,
                };
                userManager.Create(userToInsert, "Bt@Admin1!");
                //userManager.AddToRole(userToInsert.Id, "Admin");
                userManager.AddToRole(userToInsert.Id, "Demo_Admin");
            }

            if (!context.Users.Any(u => u.UserName == "DemoDev@venellbugtracker.com"))
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var userToInsert = new ApplicationUser
                {
                    UserName = "DemoDev@venellbugtracker.com",
                    DeveloperID = 3,
                    FirstName = "DemoD",
                    LastName = "Dev",
                    PhoneNumber = "8325856877",
                    PhoneNumberConfirmed = true,
                    Email = "DemoDev@venellbugtracker.com",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    Authorized = true,
                    Approved = true,
                    UserType = UserType.Employee,
                    DemoAccount = true,
                };
                userManager.Create(userToInsert, "Bt@d3velop3r1!!");
                //userManager.AddToRole(userToInsert.Id, "Developer");
                userManager.AddToRole(userToInsert.Id, "Demo_Developer");
            }

            if (!context.Users.Any(u => u.UserName == "DemoPM@venellbugtracker.com"))
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var userToInsert = new ApplicationUser
                {
                    UserName = "DemoPM@venellbugtracker.com",
                    DeveloperID = 4,
                    FirstName = "DemoD",
                    LastName = "PM",
                    PhoneNumber = "8325856877",
                    PhoneNumberConfirmed = true,
                    Email = "DemoPM@venellbugtracker.com",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    Authorized = true,
                    Approved = true,
                    UserType = UserType.Employee,
                    DemoAccount = true,
                };
                userManager.Create(userToInsert, "Bt@projectM@nag3r1!");
                //userManager.AddToRole(userToInsert.Id, "Project Manager");
                userManager.AddToRole(userToInsert.Id, "Demo_ProjectManager");
            }

            if (!context.Users.Any(u => u.UserName == "DemoSubmit@venellbugtracker.com"))
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var userToInsert = new ApplicationUser
                {
                    UserName = "DemoSubmit@venellbugtracker.com",
                    SubmitterID = 1,
                    FirstName = "DemoS",
                    LastName = "Submit",
                    PhoneNumber = "8325856877",
                    PhoneNumberConfirmed = true,
                    Email = "DemoSubmit@venellbugtracker.com",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    Authorized = true,
                    Approved = true,
                    UserType = UserType.Customer,
                    DemoAccount = true,
                };
                userManager.Create(userToInsert, "Bt@Submitt3r1!");
                //userManager.AddToRole(userToInsert.Id, "Basic");
                userManager.AddToRole(userToInsert.Id, "Demo_Submitter");
            }





            //-- AddOrUpdateDeveloper --//
            AddOrUpdateDeveloper(context, "Great work", "PM");
            //AddOrUpdateDeveloper(context, "Great work", "Davis"); //NOTE: This was causing an error of having 2 Developers assigned to single ticket for some reson. Commenting out to check. -EV 02/09/2021...
            AddOrUpdateDeveloper(context, "Release bug: Incorrect Date/Time", "Dev");
            AddOrUpdateDeveloper(context, "Aesthetics please", "Dev");

            AddOrUpdateDeveloper(context, "What's new Scoobydoo release 1.1.12", "PM");
            AddOrUpdateDeveloper(context, "Migration Failure - P1", "PM");
            AddOrUpdateDeveloper(context, "EmailAddress field limit too short", "PM");
            AddOrUpdateDeveloper(context, "Still cannot access the SQL Database", "Admin");

            context.SaveChanges();
            //------//

            //-- AddOrUpdateAppUser --//
            //AddOrUpdateAppUser(context, "calexander@gmail.com", "calexander@gmail.com");
            //AddOrUpdateAppUser(context, "malonzo@yahoo.com", "malonzo@yahoo.com");
            //AddOrUpdateAppUser(context, "aanand@@sbglobal.net", "aanand@@sbglobal.net");
            //AddOrUpdateAppUser(context, "gbarzdukas@outlook.com", "gbarzdukas@outlook.com");

            //AddOrUpdateAppUser(context, "yli@gmail.com", "yli@gmail.com");
            //AddOrUpdateAppUser(context, "pjustice@outlook.com", "pjustice@outlook.com");
            //AddOrUpdateAppUser(context, "lnorman@gmail.com", "lnorman@gmail.com");
            //AddOrUpdateAppUser(context, "nolivettot@sbcglobal.net", "nolivettot@sbcglobal.net");

            //AddOrUpdateAppUser(context, "DemoSubmitEV@Mailinator.com", "DemoSubmitEV@Mailinator.com");
            //AddOrUpdateAppUser(context, "evenell@bnbank.bank", "evenell@bnbank.bank");
            //AddOrUpdateAppUser(context, "bdavis@core-techs.net", "bdavis@core-techs.net");
            //AddOrUpdateAppUser(context, "developerCF@mailinator.com", "developerCF@mailinator.com");

            //AddOrUpdateAppUser(context, "arcadia_sin@mailinator.com", "arcadia_sin@mailinator.com");
            //AddOrUpdateAppUser(context, "projectmanagerCF@mailinator.com", "projectmanagerCF@mailinator.com");
            //AddOrUpdateAppUser(context, "DemoAdminEV@Mailinator.com", "DemoAdminEV@Mailinator.com");
            //AddOrUpdateAppUser(context, "DemoDevEV@Mailinator.com", "DemoDevEV@Mailinator.com");
            //AddOrUpdateAppUser(context, "DemoPMEV@Mailinator.com", "DemoPMEV@Mailinator.com");

            //context.SaveChanges();
            //------//


            //-- enrollments --//
            var enrollments = new List<Enrollment>
            {
                new Enrollment {
                    SubmitterID = submitters.Single(s => s.LastName == "Submit").ID,
                    TicketID = tickets.Single(c => c.TicketTitle == "Great work").TicketID,
                    //Grade = Grade.A
                },

                new Enrollment {
                    SubmitterID = submitters.Single(s => s.LastName == "Submit").ID,
                    TicketID = tickets.Single(c => c.TicketTitle == "Release bug: Incorrect Date/Time").TicketID,
                    //Grade = Grade.C
                },

                new Enrollment {
                    SubmitterID = submitters.Single(s => s.LastName == "Submit").ID,
                    TicketID = tickets.Single(c => c.TicketTitle == "Aesthetics please").TicketID,
                    //Grade = Grade.B
                },

                new Enrollment {
                    SubmitterID = submitters.Single(s => s.LastName == "Submit").ID,
                    TicketID = tickets.Single(c => c.TicketTitle == "What's new Scoobydoo release 1.1.12").TicketID,
                    //Grade = Grade.B
                },

                new Enrollment {
                    SubmitterID = submitters.Single(s => s.LastName == "Submit").ID,
                    TicketID = tickets.Single(c => c.TicketTitle == "Migration Failure - P1").TicketID,
                    //Grade = Grade.B
                },

                new Enrollment {
                    SubmitterID = submitters.Single(s => s.LastName == "Submit").ID,
                    TicketID = tickets.Single(c => c.TicketTitle == "EmailAddress field limit too short").TicketID,
                    //Grade = Grade.B
                },

                new Enrollment {
                    SubmitterID = submitters.Single(s => s.LastName == "Submit").ID,
                    TicketID = tickets.Single(c => c.TicketTitle == "Great work").TicketID,
                    //Grade = Grade.B
                },

                new Enrollment {
                    SubmitterID = submitters.Single(s => s.LastName == "Submit").ID,
                    TicketID = tickets.Single(c => c.TicketTitle == "Release bug: Incorrect Date/Time").TicketID,
                    //Grade = Grade.B
                },

                new Enrollment {
                    SubmitterID = submitters.Single(s => s.LastName == "Submit").ID,
                    TicketID = tickets.Single(c => c.TicketTitle == "Great work").TicketID,
                    //Grade = Grade.B
                },

                new Enrollment {
                    SubmitterID = submitters.Single(s => s.LastName == "Submit").ID,
                    TicketID = tickets.Single(c => c.TicketTitle == "EmailAddress field limit too short").TicketID,
                    //Grade = Grade.B
                },

                new Enrollment {
                    SubmitterID = submitters.Single(s => s.LastName == "Submit").ID,
                    TicketID = tickets.Single(c => c.TicketTitle == "Still cannot access the SQL Database").TicketID,
                    //Grade = Grade.B
                },
            };

            foreach (Enrollment e in enrollments)
            {
                var enrollmentInDataBase = context.Enrollments.Where(
                    s =>
                        s.Submitter.ID == e.SubmitterID &&
                        s.Ticket.TicketID == e.TicketID).SingleOrDefault();
                if (enrollmentInDataBase == null)
                {
                    context.Enrollments.Add(e);
                }
            }
            context.SaveChanges();
            //------//
        }
        void AddOrUpdateDeveloper(ApplicationContext context, string ticketTitle, string developerName)
        {
            var crs = context.Tickets.SingleOrDefault(c => c.TicketTitle == ticketTitle);
            var inst = crs.Developers.SingleOrDefault(i => i.LastName == developerName);
            if (inst == null)
                crs.Developers.Add(context.Developers.Single(i => i.LastName == developerName));
        }
        
        

        //void AddOrUpdateBuiltInUsers(ApplicationContext context, string UserName, string RoleName, ApplicationUser user)
        //{
        //    var crs = context.Users.SingleOrDefault(c => c.UserName == UserName);
        //    var inst = crs.Roles.SingleOrDefault(i => i.UserId == user.Id);
        //    if (inst == null)
        //        crs.Roles.Add(context.Roles.Single(i => i.))
        //}
    }
}
