using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTrackerApplication.DAL;
using BugTrackerApplication.ViewModels;
using BugTrackerApplication.Models;
using System.Data;
using System.Text;
using System.Web.Services;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Sql;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;


namespace BugTrackerApplication.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext db = new ApplicationContext();
        public JsonResult BarChartData1()
        {
            // Bar Chart - Tickets by Priority//
            //--------------------------------//

            using (var context = new ApplicationContext())
            {
                var TPData = context.Tickets.Select(t => t.TicketPriority).ToList();
                var TP_Low = context.Tickets
                    .Where(i => i.TicketPriority == TicketPriority.Low)
                    .Select(group => new
                    {
                        LowTicketPriority = group.TicketPriority == TicketPriority.Low,
                        //Count_Low = TP_Low.Count()
                    });

                var TP_Medium = context.Tickets
                    .Where(i => i.TicketPriority == TicketPriority.Medium)
                    .Select(group => new
                    {
                        MedTicketPriority = group.TicketPriority == TicketPriority.Medium,
                    });

                var TP_High = context.Tickets
                    .Where(i => i.TicketPriority == TicketPriority.High)
                    .Select(group => new
                    {
                        HighTicketPriority = group.TicketPriority == TicketPriority.High,
                    });


                BarChart _chart = new BarChart();
                _chart.labels = new string[] { "Low", "Medium", "High" };
                _chart.datasets = new List<Datasets>();
                List<Datasets> _dataSet = new List<Datasets>();
                _dataSet.Add(new Datasets()
                {
                    label = "Ticket Count: ",
                    //data = new int[] { TP_Low.Count() },
                    data = new int[] { TP_Low.Count(), TP_Medium.Count(), TP_High.Count() },
                    backgroundColor = new string[] { "#FF4500", "#FF0000", "#900000" },
                    borderColor = new string[] { "#FF4500", "#FF0000", "#900000" },
                    borderWidth = "1"

                });
                _chart.datasets = _dataSet;
                return Json(_chart, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult BarChartData2()
        {
            //---------------------------------//
            //- Bar Chart - Tickets by Status -//
            //---------------------------------//
            using (var context = new ApplicationContext())
            {
                var TSData = context.Tickets.Select(t => t.TicketStatus).ToList();
                var TS_New = context.Tickets
                    .Where(i => i.TicketStatus == TicketStatus.New)
                    .Select(group => new
                    {
                        NewTicketStatus = group.TicketStatus == TicketStatus.New
                    });

                var TS_InProgress = context.Tickets
                    .Where(i => i.TicketStatus == TicketStatus.InProgress)
                    .Select(group => new
                    {
                        NewTicketStatus = group.TicketStatus == TicketStatus.InProgress
                    });

                var TS_RequestInfo = context.Tickets
                    .Where(i => i.TicketStatus == TicketStatus.RequestedInfo)
                    .Select(group => new
                    {
                        NewTicketStatus = group.TicketStatus == TicketStatus.RequestedInfo
                    });

                var TS_Scheduled = context.Tickets
                    .Where(i => i.TicketStatus == TicketStatus.Scheduled)
                    .Select(group => new
                    {
                        NewTicketStatus = group.TicketStatus == TicketStatus.Scheduled
                    });

                var TS_WaitingOnUser = context.Tickets
                    .Where(i => i.TicketStatus == TicketStatus.WaitingOnUser)
                    .Select(group => new
                    {
                        NewTicketStatus = group.TicketStatus == TicketStatus.WaitingOnUser
                    });

                var TS_Cancelled = context.Tickets
                    .Where(i => i.TicketStatus == TicketStatus.Cancelled)
                    .Select(group => new
                    {
                        NewTicketStatus = group.TicketStatus == TicketStatus.Cancelled
                    });

                var TS_Closed = context.Tickets
                    .Where(i => i.TicketStatus == TicketStatus.Closed)
                    .Select(group => new
                    {
                        NewTicketStatus = group.TicketStatus == TicketStatus.Closed
                    });

                var TS_Escalation = context.Tickets
                    .Where(i => i.TicketStatus == TicketStatus.Escalation)
                    .Select(group => new
                    {
                        NewTicketStatus = group.TicketStatus == TicketStatus.Escalation
                    });

                BarChart _tsChart = new BarChart();
                _tsChart.labels = new string[] { "New", "In Progress", "Waiting on User", "Resolved", "Additional Info Requested" };
                _tsChart.datasets = new List<Datasets>();

                //List<double> dataList = new List<double>();

                List<Datasets> _tsdataSet = new List<Datasets>();
                _tsdataSet.Add(new Datasets()
                {
                    label = "Ticket Status: ",
                    //data = new int[] { TS_New.Count(), TS_InProgress.Count(), TS_RequestInfo.Count(), TS_Scheduled.Count(), TS_WaitingOnUser.Count(), TS_Cancelled.Count(), TS_Closed.Count(), TS_Escalation.Count() },
                    //backgroundColor = new string[] { "#000080", "#FFC107", "#0000CD", "#4169E1", "#6A5ACD", "#F0F8FF", "#2E8B57", "#800000" },
                    //borderColor = new string[] { "#000080", "#FFC107", "#0000CD","#4169E1", "#6A5ACD", "#F0F8FF", "#2E8B57", "#800000" },
                    data = new int[] { TS_New.Count(), TS_InProgress.Count(), TS_WaitingOnUser.Count(), TS_Closed.Count(), TS_RequestInfo.Count() },
                    backgroundColor = new string[] { "#00008B", "#0000CD", "#4169E1", "#1E90FF", "#483D8B" },
                    borderColor = new string[] { "#00008B", "#0000CD", "#4169E1", "#1E90FF", "#483D8B" },
                    borderWidth = "1"
                });
                _tsChart.datasets = _tsdataSet;
                return Json(_tsChart, JsonRequestBehavior.AllowGet);


            }
        }

        //----------------------------------//
        //- Donut Chart - Tickets by Type -//
        //---------------------------------//


        public JsonResult DonutChartData1()
        {

            using (var context = new ApplicationContext())
            {
                //var TTData = context.Tickets.Select(t => t.TicketType).ToList();
                var TTData = context.Tickets.Select(t => t.TicketType).ToArray();
                var TT_Bugs = context.Tickets
                    .Where(i => i.TicketType == TicketType.Bugs)
                    .Select(group => new
                    {
                        BugsTicketType = group.TicketType == TicketType.Bugs,
                    });

                var TT_Errors = context.Tickets
                    .Where(i => i.TicketType == TicketType.Errors)
                    .Select(group => new
                    {
                        BugsTicketType = group.TicketType == TicketType.Errors,
                    });

                var TT_FeatureRequests = context.Tickets
                    .Where(i => i.TicketType == TicketType.FeatureRequest)
                    .Select(group => new
                    {
                        BugsTicketType = group.TicketType == TicketType.FeatureRequest,
                    });

                var TT_OtherComments = context.Tickets
                    .Where(i => i.TicketType == TicketType.OtherComments)
                    .Select(group => new
                    {
                        OtherCommentTicketType = group.TicketType == TicketType.OtherComments,
                    });

                var TT_TrainDocReq = context.Tickets
                    .Where(i => i.TicketType == TicketType.TrainingDocumentRequests)
                    .Select(group => new
                    {
                        TrainDocReqTicketType = group.TicketType == TicketType.TrainingDocumentRequests,
                    });

                //var TTBugs_Count = TT_Bugs.Count();
                //var TTErrors_Count = TT_Errors.Count();
                //var TTFeatReqs_Count = TT_FeatureRequests.Count();

                BarChart _ttChart = new BarChart();
                _ttChart.labels = new string[] { "Bugs", "Errors", "Feature Requests", "Other Comments", "Training Document Request" };
                _ttChart.datasets = new List<Datasets>();
                List<Datasets> _ttdataSet = new List<Datasets>();
                _ttdataSet.Add(new Datasets()
                {
                    label = "Ticket Type: ",
                    data = new int[] { TT_Bugs.Count(), TT_Errors.Count(), TT_FeatureRequests.Count(), TT_OtherComments.Count(), TT_TrainDocReq.Count() },
                    backgroundColor = new string[] { "#FF4500", "#8000000", "#9400D3", "#228B22", "#FF8C00" },//, "#FF0000", "#9000000" },
                    borderColor = new string[] { "#FF4500", "#8000000", "#9400D3", "#228B22", "#FF8C00" },//, "#FF0000", "#(000000" },
                    //NOTE: I DID IT!!! Just need to change the dark grey color for ERRORS to a dark red. -EV 03/31/2021...
                    borderWidth = "1"
                });
                _ttChart.datasets = _ttdataSet;

                return Json(_ttChart, JsonRequestBehavior.AllowGet);
            }
        }

        //-------------------------------------//

        
        //----------------------------------//
        //public JsonResult TicketByProfileData()
        public JsonResult TicketLifeTimeChartData()
        {
            using (var context = new ApplicationContext())
            {
                var TicketData = context.Tickets
                    .Select(i => i.Created)
                    //.Where(c => c.Ticks < System.DateTime.Now.Month)
                    .Where(c => c.Month > 0)
                    .Select(group => new
                    {
                        //TicketLifeTime = group.Ticks,
                        January = group.Month == 1,
                        February = group.Month == 2,
                        March = group.Month == 3,
                        April = group.Month == 4, 
                        May = group.Month == 5,
                        June = group.Month == 6,
                        July = group.Month == 7,
                        August = group.Month == 8,
                        September = group.Month == 9,
                        October = group.Month == 10,
                        November = group.Month == 11,
                        December = group.Month == 12,
                    });

                //var todaysDateData = System.DateTime.Now.Ticks;
                //var TicketData = context.Tickets
                //    .Where(i => i.Created.Ticks < todaysDateData)
                //    .Select(group => new
                //    {
                //        TicketLifeTime = group.Created.Ticks,
                //    });

                BarChart _tickLifeChart = new BarChart();
                _tickLifeChart.labels = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                _tickLifeChart.datasets = new List<Datasets>();
                List<Datasets> _tldataSet = new List<Datasets>();
                _tldataSet.Add(new Datasets()
                {
                    label = "Overall Ticket Lifetime: ",
                    data = new int[] { TicketData.Where(i => i.January).Count(), TicketData.Where(i => i.February).Count(), TicketData.Where(i => i.March).Count(), TicketData.Where(i => i.April).Count(), TicketData.Where(i => i.May).Count(), TicketData.Where(i => i.June).Count(), TicketData.Where(i => i.July).Count(), TicketData.Where(i => i.August).Count(), TicketData.Where(i => i.September).Count(), TicketData.Where(i => i.October).Count(), TicketData.Where(i => i.November).Count(), TicketData.Where(i => i.December).Count() },
                    //backgroundColor = new string[] { "#FF4500", "#8000000", "#9400D3" },
                    backgroundColor = new string[] { "#007BFF", "#FFC107", "#007BFF", "#FFC107", "#228B22", "#FF4500", "#FF0000", "#228B22", "FF0000", "#FF4500", "FF0000", "#228B22" },
                    borderColor = new string[] { "#007BFF", "#FFC107", "#007BFF", "#FFC107", "#228B22", "#FF4500", "#FF0000", "#228B22", "FF0000", "#FF4500", "FF0000", "#228B22" },
                    //borderColor = new string[] { "#FF4500", "#ffc107", "#007bft" },
                    borderWidth = "1"
                });
                _tickLifeChart.datasets = _tldataSet;

                return Json(_tickLifeChart, JsonRequestBehavior.AllowGet);
            }
        }
        //----------------------------------//

        //}
        public ActionResult Index(string searchString)
        {
            var projects = from p in db.Projects
                           select p;
            var tickets = from t in db.Tickets
                          select t;
            var developers = from d in db.Developers
                             select d;
            var submitters = from s in db.Submitters
                             select s;
            var people = from users in db.People
                         select users;
            var appUsers = from aU in db.AppUsers
                           select aU;

            if (!String.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(p => p.ProjectName.Contains(searchString)
                                        || p.ProjectDescription.Contains(searchString)
                                        || p.AssignedDeveloper.Contains(searchString));

                tickets = tickets.Where(t => t.TicketTitle.Contains(searchString)
                                        || t.TicketDescription.Contains(searchString));

                developers = developers.Where(d => d.FirstMidName.Contains(searchString)
                                        || d.LastName.Contains(searchString)
                                        || d.FullName.Contains(searchString)
                                        || d.EmailAddress.Contains(searchString));

                submitters = submitters.Where(s => s.FirstMidName.Contains(searchString)
                                        || s.LastName.Contains(searchString)
                                        || s.FullName.Contains(searchString)
                                        || s.EmailAddress.Contains(searchString));
                appUsers = appUsers.Where(user => user.FirstName.Contains(searchString)
                                        || user.LastName.Contains(searchString)
                                        || user.FullName.Contains(searchString)
                                        || user.Email.Contains(searchString)
                                        || user.JobTitle.Contains(searchString)
                                        || user.Location.Contains(searchString)
                                        || user.Skills.Contains(searchString)
                                        || user.UserName.Contains(searchString)
                                        || user.DisplayName.Contains(searchString)
                                        || user.EducationOrExperience.Contains(searchString));
            }

            return View();
        }

        //-- Original index() Code --//
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult About()
        {
            IQueryable<EnrollmentDateGroup> data = from submitter in db.Submitters
                                                   group submitter by submitter.EnrollmentDate into dateGroup
                                                   select new EnrollmentDateGroup()
                                                   {
                                                       EnrollmentDate = dateGroup.Key,
                                                       SubmitterCount = dateGroup.Count()
                                                   };
            return View(data.ToList());
        }

        public ActionResult Contact()
        {
            
            ViewBag.Message = "Send us a message to tell us what you think!";

            return View();


            //return View();
        }

        public ActionResult PageDevelopment()
        {

            return View();
        }


        //-- Original Contact() Code --//
        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}