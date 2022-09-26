namespace BugTrackerApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AppUserDemoAccountPropertyAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppUser", "DemoAccount", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppUser", "DemoAccount");
        }
    }
}
