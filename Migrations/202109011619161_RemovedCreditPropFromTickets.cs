namespace BugTrackerApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedCreditPropFromTickets : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Ticket", "Credits");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Ticket", "Credits", c => c.Int(nullable: false));
        }
    }
}
