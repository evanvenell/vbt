namespace BugTrackerApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inheritance : DbMigration
    {
        public override void Up()
        {
            // Drop foreign keys and indexes that point to tables we're going to drop.
            DropForeignKey("dbo.Enrollment", "SubmitterID", "dbo.Submitter");
            DropIndex("dbo.Enrollment", new[] { "SubmitterID" });

            RenameTable(name: "dbo.Developer", newName: "Person");
            AddColumn("dbo.Person", "EnrollmentDate", c => c.DateTime());
            AddColumn("dbo.Person", "Discriminator", c => c.String(nullable: false, maxLength: 128, defaultValue: "Developer"));
            AlterColumn("dbo.Person", "HireDate", c => c.DateTime());
            AddColumn("dbo.Person", "OldId", c => c.Int(nullable: true));

            // Copy existing Submitter data into new Person table.
            Sql("INSERT INTO dbo.Person (LastName, FirstName, HireDate, EnrollmentDate, Discriminator, OldId) SELECT LastName, FirstName, null AS HireDate, EnrollmentDate, 'Submitter' AS Discriminator, ID AS OldId FROM dbo.Submitter");

            // Fix up existing relationships to match new PK's.
            Sql("UPDATE dbo.Enrollment SET SubmitterId = (SELECT ID FROM dbo.Person WHERE OldId = Enrollment.SubmitterId AND Discriminator = 'Submitter')");

            // Remove temporary key
            DropColumn("dbo.Person", "OldId");

            DropTable("dbo.Submitter");

            // Re-create foreign keys and indexes pointing to new table.
            AddForeignKey("dbo.Enrollment", "SubmitterID", "dbo.Person", "ID", cascadeDelete: true);
            CreateIndex("dbo.Enrollment", "SubmitterID");


            //-- Original Up() Method Code --//
            //RenameTable(name: "dbo.Developer", newName: "Person");
            //AddColumn("dbo.Person", "EnrollmentDate", c => c.DateTime());
            //AddColumn("dbo.Person", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            //AlterColumn("dbo.Person", "HireDate", c => c.DateTime());
            //DropTable("dbo.Submitter");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Submitter",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LastName = c.String(maxLength: 50),
                        FirstName = c.String(maxLength: 50),
                        EmailAddress = c.String(),
                        EnrollmentDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AlterColumn("dbo.Person", "HireDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Person", "Discriminator");
            DropColumn("dbo.Person", "EnrollmentDate");
            RenameTable(name: "dbo.Person", newName: "Developer");
        }
    }
}
