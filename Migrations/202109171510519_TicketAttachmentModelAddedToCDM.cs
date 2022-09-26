namespace BugTrackerApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TicketAttachmentModelAddedToCDM : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketAttachments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TicketID = c.Int(nullable: false),
                        Title = c.String(),
                        FileName = c.String(),
                        Author = c.String(),
                        Uploaded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Ticket", t => t.TicketID, cascadeDelete: true)
                .Index(t => t.TicketID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketAttachments", "TicketID", "dbo.Ticket");
            DropIndex("dbo.TicketAttachments", new[] { "TicketID" });
            DropTable("dbo.TicketAttachments");
        }
    }
}
