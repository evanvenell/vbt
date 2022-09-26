using BugTrackerApplication.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace BugTrackerApplication.DAL
{
    //public class ApplicationContext : DbContext
    //public class ApplicationContext : IdentityDbContext<AppUser>
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationContext() : base("ApplicationContext") //NOTE: here is the <connectionStrings>...<connectionStrings> code block name...-EV 04/26/21...
        {

        }


        public DbSet<Submitter> Submitters { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Developer> Developers { get; set; }
        //public DbSet<RoleAssignment> RoleAssignments { get; set; } //NOTE: Will add back later with Identity...maybe. -EV 06/14/2021...
        public DbSet<Project> Projects { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<TicketAttachments> TicketAttachments { get; set; }

        public DbSet<Person> People { get; set; }

        public DbSet<AppUser> AppUsers { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //--- Identity Integration --//

            //-----------The Below Works --//
            //modelBuilder.Entity<IdentityUser>()
            //    .ToTable("AspNetUsers");
            //modelBuilder.Entity<AppUser>()
            //    .ToTable("AspNetUsers"); //NOTE: This causes an error where APplicationUSer is not part of currentcontext...

            //modelBuilder.Entity<AppRole>()
            //    .ToTable("Roles");

            //modelBuilder.Entity<Person>() //NOTE: Leave this on here because with the above and my Nav props, the table gets auto generated anyways. -EV 06/15/2021...
            //    .HasMany(c => c.AppUsers).WithMany(i => i.People)
            //    .Map(t => t.MapLeftKey("PersonID")
            //        .MapRightKey("AppUserID")
            //        .ToTable("AppPeople"));
            //------------------------------//
            //modelBuilder.Entity<AppUser>()
            //modelBuilder.Entity<IdentityUser>()
            //    .ToTable("AspNetUsers"); //NOTE: This causes an error where ApplicationUser is not part of currentcontext...

            //modelBuilder.Entity<AppRole>()
            //    .ToTable("Roles");

            //modelBuilder.Entity<Person>() //NOTE: Leave this on here because with the above and my Nav props, the table gets auto generated anyways. -EV 06/15/2021...
            //    .HasMany(c => c.AppUsers).WithMany(i => i.People)
            //    .Map(t => t.MapLeftKey("PersonID")
            //        .MapRightKey("AppUserID")
            //        .ToTable("AppPeople"));
            //--- ^^^ Latest Attempt - Partially Successful ^^^--//
            //modelBuilder.Entity<ApplicationUser>()
            //    .HasMany(c => c.People).WithMany(i => i.AppUsers)

            modelBuilder.Entity<AppUser>()
                .HasMany(c => c.People).WithMany(i => i.AppUsers)
                .Map(t => t.MapLeftKey("AppUserId")
                    .MapRightKey("PersonID")
                    .ToTable("AppPeople"));


            //---------------------------//

            //--- Original modelBuilder Code --//
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Ticket>()
                .HasMany(c => c.Developers).WithMany(i => i.Tickets)
                .Map(t => t.MapLeftKey("TicketID")
                    .MapRightKey("DeveloperID")
                    .ToTable("TicketDeveloper"));


            modelBuilder.Entity<Project>().MapToStoredProcedures();

            modelBuilder.Entity<Project>()
                .Property(p => p.RowVersion).IsConcurrencyToken();


        }

        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }


    }


    //-- Original ApplicationContext Code Pre-Identity --//
    //public class ApplicationContext : DbContext //NOTE: This is what to change to use OWIN. -EV 04/26/2021...
    ////public class ApplicationContext : IdentityDbContext<ApplicationUser>
    //{

    //    public ApplicationContext() : base("ApplicationContext") //NOTE: here is the <connectionStrings>...<connectionStrings> code block name...-EV 04/26/21...
    //    {

    //    }


    //    public DbSet<Submitter> Submitters { get; set; }
    //    public DbSet<Enrollment> Enrollments { get; set; }
    //    public DbSet<Ticket> Tickets { get; set; }
    //    public DbSet<Developer> Developers { get; set; }
    //    //public DbSet<RoleAssignment> RoleAssignments { get; set; } //NOTE: Will add back later with Identity...maybe. -EV 06/14/2021...
    //    public DbSet<Project> Projects { get; set; }

    //    public DbSet<Comment> Comments { get; set; }

    //    //public DbSet<AppUser> AppUsers { get; set; }
    //    //public DbSet<AppUserType> AppUserTypes { get; set; }

    //    public DbSet<Person> People { get; set; }

    //    //public DbSet<AppProfile> AppProfiles { get; set; }

    //    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //    {
    //        base.OnModelCreating(modelBuilder);
    //        modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

    //        //modelBuilder.Entity<AppUserType>()
    //        //    .HasKey(k => k.);



    //        //-- AppProfile --//
    //        //modelBuilder.Entity<AppProfile>()
    //        //    .HasMany(c => c.People).WithMany(i => i.AppProfiles)
    //        //    .Map(t => t.MapLeftKey("UserID")
    //        //        .MapRightKey("PersonID")
    //        //        .ToTable("ProfileDatabase"));

    //        //----------------//

    //        modelBuilder.Entity<Ticket>()
    //            .HasMany(c => c.Developers).WithMany(i => i.Tickets)
    //            .Map(t => t.MapLeftKey("TicketID")
    //                .MapRightKey("DeveloperID")
    //                .ToTable("TicketDeveloper"));

    //        modelBuilder.Entity<Project>().MapToStoredProcedures();

    //        modelBuilder.Entity<Project>()
    //            .Property(p => p.RowVersion).IsConcurrencyToken();

    //    }
    //    //--------------------------------------------//
    //    public static ApplicationContext Create()
    //    {
    //       return new ApplicationContext();
    //    }
    //    //--------------------------------------------//

    //}

}