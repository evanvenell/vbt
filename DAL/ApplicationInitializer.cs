using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using BugTrackerApplication.Models;

namespace BugTrackerApplication.DAL
{
    public class ApplicationInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<ApplicationContext>
    {
        protected override void Seed(ApplicationContext context)
        {
            //-- submitters --//
            var submitters = new List<Submitter>
            {
                new Submitter{FirstMidName="Carson",LastName="Alexander", EmailAddress = "calexander@gmail.com",EnrollmentDate=DateTime.Parse("2021-01-13")},
                new Submitter{FirstMidName="Meredith",LastName="Alonso", EmailAddress = "malonzo@yahoo.com",EnrollmentDate=DateTime.Parse("2021-01-13")},
                new Submitter{FirstMidName="Arturo",LastName="Anand", EmailAddress = "aanand@sbcglobal.net",EnrollmentDate=DateTime.Parse("2021-01-13")},
                new Submitter{FirstMidName="Gytis",LastName="Barzdukas", EmailAddress = "gbarzdukas@outlook.com",EnrollmentDate=DateTime.Parse("2021-01-13")},
                new Submitter{FirstMidName="Yan",LastName="Li", EmailAddress = "yli@gmail.com",EnrollmentDate=DateTime.Parse("2021-01-13")},
                new Submitter{FirstMidName="Peggy",LastName="Justice", EmailAddress = "pjustice@outlook.com",EnrollmentDate=DateTime.Parse("2021-01-13")},
                new Submitter{FirstMidName="Laura",LastName="Norman", EmailAddress = "lnorman@gmail.com",EnrollmentDate=DateTime.Parse("2021-01-13")},
                new Submitter{FirstMidName="Nino",LastName="Olivetto", EmailAddress = "nolivettot@sbcglobal.net",EnrollmentDate=DateTime.Parse("2021-01-13")}
            };
            submitters.ForEach(s => context.Submitters.Add(s));
            context.SaveChanges();
            //------//


            //-- tickets --//
            var tickets = new List<Ticket>
            {
                new Ticket{TicketID=1050, TicketTitle = "Great work", }, //Credits=3, },
                new Ticket{TicketID=4022, TicketTitle = "Release Bug: Incorrect Date/Time",}, // Credits=3, },
                new Ticket{TicketID=4041, TicketTitle = "Aesthetics please",}, // Credits=3, },
                new Ticket{TicketID=1045, TicketTitle = "What's new Scoobydoo release 1.1.12",}, // Credits=4, },
                new Ticket{TicketID=3141, TicketTitle = "Migration Failure - P1",}, // Credits=4, },
                new Ticket{TicketID=2021, TicketTitle = "EmailAddress field limit too short",}, // Credits=3, },
                new Ticket{TicketID=2042, TicketTitle = "Still cannot access the SQL Database",}, // Credits=4, },
            };

            tickets.ForEach(s => context.Tickets.Add(s));
            context.SaveChanges();
            //------//


            //-- enrollments --//
            var enrollments = new List<Enrollment>
            {
                new Enrollment{SubmitterID=1, TicketID=1050, }, //Grade=Grade.A },
                new Enrollment { SubmitterID = 1, TicketID = 4022, }, //Grade = Grade.C },
                new Enrollment{SubmitterID=1, TicketID=4041, }, //Grade=Grade.B },
                new Enrollment { SubmitterID = 2, TicketID = 1045, }, //Grade=Grade.B },
                new Enrollment { SubmitterID = 2, TicketID = 3141, }, //Grade=Grade.F },
                new Enrollment { SubmitterID = 2, TicketID = 2021, }, //Grade=Grade.F },
                new Enrollment{SubmitterID=3, TicketID=1050 },
                new Enrollment{SubmitterID=4, TicketID=1050 },
                new Enrollment { SubmitterID = 4, TicketID = 4022, }, //Grade=Grade.F },
                new Enrollment { SubmitterID = 5, TicketID = 4041, }, //Grade=Grade.C },
                new Enrollment{SubmitterID=6, TicketID=1045 },
                new Enrollment { SubmitterID = 7, TicketID = 3141, }, //Grade=Grade.A },
            };

            enrollments.ForEach(s => context.Enrollments.Add(s));
            context.SaveChanges();
            //------//
        }
    }
}