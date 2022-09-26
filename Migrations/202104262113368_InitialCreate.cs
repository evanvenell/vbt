namespace BugTrackerApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Enrollment",
                c => new
                    {
                        EnrollmentID = c.Int(nullable: false, identity: true),
                        TicketID = c.Int(nullable: false),
                        SubmitterID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EnrollmentID)
                .ForeignKey("dbo.Submitter", t => t.SubmitterID, cascadeDelete: true)
                .ForeignKey("dbo.Ticket", t => t.TicketID, cascadeDelete: true)
                .Index(t => t.TicketID)
                .Index(t => t.SubmitterID);
            
            CreateTable(
                "dbo.Submitter",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LastName = c.String(),
                        FirstMidName = c.String(),
                        Email = c.String(),
                        EnrollmentDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Ticket",
                c => new
                    {
                        TicketID = c.Int(nullable: false),
                        TicketTitle = c.String(),
                        TicketDescription = c.String(),
                        Submitter = c.String(),
                        TicketPriority = c.Int(),
                        TicketStatus = c.Int(),
                        TicketType = c.Int(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Credits = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TicketID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Enrollment", "TicketID", "dbo.Ticket");
            DropForeignKey("dbo.Enrollment", "SubmitterID", "dbo.Submitter");
            DropIndex("dbo.Enrollment", new[] { "SubmitterID" });
            DropIndex("dbo.Enrollment", new[] { "TicketID" });
            DropTable("dbo.Ticket");
            DropTable("dbo.Submitter");
            DropTable("dbo.Enrollment");
        }
    }
}
