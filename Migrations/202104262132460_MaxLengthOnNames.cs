namespace BugTrackerApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MaxLengthOnNames : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Submitter", "LastName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Submitter", "FirstMidName", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Submitter", "FirstMidName", c => c.String());
            AlterColumn("dbo.Submitter", "LastName", c => c.String());
        }
    }
}
