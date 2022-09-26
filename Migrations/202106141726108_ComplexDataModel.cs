namespace BugTrackerApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ComplexDataModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comment",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TicketID = c.Int(nullable: false),
                        CommentTitle = c.String(),
                        Commenter = c.String(),
                        Message = c.String(),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Ticket", t => t.TicketID, cascadeDelete: true)
                .Index(t => t.TicketID);
            
            CreateTable(
                "dbo.Developer",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LastName = c.String(nullable: false, maxLength: 50),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        Email = c.String(),
                        HireDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Project",
                c => new
                    {
                        ProjectID = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(maxLength: 50),
                        ProjectDescription = c.String(),
                        AssignedDeveloper = c.String(),
                        Budget = c.Decimal(nullable: false, storeType: "money"),
                        StartDate = c.DateTime(nullable: false),
                        DeveloperID = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectID)
                .ForeignKey("dbo.Developer", t => t.DeveloperID)
                .Index(t => t.DeveloperID);
            
            CreateTable(
                "dbo.TicketDeveloper",
                c => new
                    {
                        TicketID = c.Int(nullable: false),
                        DeveloperID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TicketID, t.DeveloperID })
                .ForeignKey("dbo.Ticket", t => t.TicketID, cascadeDelete: true)
                .ForeignKey("dbo.Developer", t => t.DeveloperID, cascadeDelete: true)
                .Index(t => t.TicketID)
                .Index(t => t.DeveloperID);
            
            AddColumn("dbo.Ticket", "ProjectID", c => c.Int(nullable: false));
            CreateIndex("dbo.Ticket", "ProjectID");
            AddForeignKey("dbo.Ticket", "ProjectID", "dbo.Project", "ProjectID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Ticket", "ProjectID", "dbo.Project");
            DropForeignKey("dbo.Project", "DeveloperID", "dbo.Developer");
            DropForeignKey("dbo.TicketDeveloper", "DeveloperID", "dbo.Developer");
            DropForeignKey("dbo.TicketDeveloper", "TicketID", "dbo.Ticket");
            DropForeignKey("dbo.Comment", "TicketID", "dbo.Ticket");
            DropIndex("dbo.TicketDeveloper", new[] { "DeveloperID" });
            DropIndex("dbo.TicketDeveloper", new[] { "TicketID" });
            DropIndex("dbo.Project", new[] { "DeveloperID" });
            DropIndex("dbo.Ticket", new[] { "ProjectID" });
            DropIndex("dbo.Comment", new[] { "TicketID" });
            DropColumn("dbo.Ticket", "ProjectID");
            DropTable("dbo.TicketDeveloper");
            DropTable("dbo.Project");
            DropTable("dbo.Developer");
            DropTable("dbo.Comment");
        }
    }
}
