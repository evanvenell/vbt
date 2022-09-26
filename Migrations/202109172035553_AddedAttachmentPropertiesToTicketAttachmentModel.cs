namespace BugTrackerApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAttachmentPropertiesToTicketAttachmentModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketAttachments", "File", c => c.Binary());
            AddColumn("dbo.TicketAttachments", "AttachmentType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicketAttachments", "AttachmentType");
            DropColumn("dbo.TicketAttachments", "File");
        }
    }
}
