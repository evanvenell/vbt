namespace BugTrackerApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailVarChangedAndRoleAssignmentForeignKeyRemoved : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Developer", "EmailAddress", c => c.String());
            AddColumn("dbo.Submitter", "EmailAddress", c => c.String());
            DropColumn("dbo.Developer", "Email");
            DropColumn("dbo.Submitter", "Email");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Submitter", "Email", c => c.String());
            AddColumn("dbo.Developer", "Email", c => c.String());
            DropColumn("dbo.Submitter", "EmailAddress");
            DropColumn("dbo.Developer", "EmailAddress");
        }
    }
}
