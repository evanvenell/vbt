namespace BugTrackerApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCustomAppUserProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppUser", "DeveloperID", c => c.Int(nullable: false));
            //AddColumn("dbo.AppUser", "DeveloperID", c => c.Int(nullable: true));
            AddColumn("dbo.AppUser", "SubmitterID", c => c.Int(nullable: false));
            //AddColumn("dbo.AppUser", "SubmitterID", c => c.Int(nullable: true));
            AddColumn("dbo.AppUser", "DisplayName", c => c.String());
            AddColumn("dbo.AppUser", "EducationOrExperience", c => c.String());
            AddColumn("dbo.AppUser", "Location", c => c.String());
            AddColumn("dbo.AppUser", "Skills", c => c.String());
            AddColumn("dbo.AppUser", "Notes", c => c.String());
            AddColumn("dbo.AppUser", "Employee", c => c.Boolean(nullable: false));
            AddColumn("dbo.AppUser", "JobTitle", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppUser", "JobTitle");
            DropColumn("dbo.AppUser", "Employee");
            DropColumn("dbo.AppUser", "Notes");
            DropColumn("dbo.AppUser", "Skills");
            DropColumn("dbo.AppUser", "Location");
            DropColumn("dbo.AppUser", "EducationOrExperience");
            DropColumn("dbo.AppUser", "DisplayName");
            DropColumn("dbo.AppUser", "SubmitterID");
            DropColumn("dbo.AppUser", "DeveloperID");
        }
    }
}
