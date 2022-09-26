namespace BugTrackerApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RowVersion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Project", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AlterStoredProcedure(
                "dbo.Project_Insert",
                p => new
                    {
                        ProjectName = p.String(maxLength: 50),
                        ProjectDescription = p.String(),
                        AssignedDeveloper = p.String(),
                        Budget = p.Decimal(precision: 19, scale: 4, storeType: "money"),
                        StartDate = p.DateTime(),
                        DeveloperID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Project]([ProjectName], [ProjectDescription], [AssignedDeveloper], [Budget], [StartDate], [DeveloperID])
                      VALUES (@ProjectName, @ProjectDescription, @AssignedDeveloper, @Budget, @StartDate, @DeveloperID)
                      
                      DECLARE @ProjectID int
                      SELECT @ProjectID = [ProjectID]
                      FROM [dbo].[Project]
                      WHERE @@ROWCOUNT > 0 AND [ProjectID] = scope_identity()
                      
                      SELECT t0.[ProjectID], t0.[RowVersion]
                      FROM [dbo].[Project] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ProjectID] = @ProjectID"
            );
            
            AlterStoredProcedure(
                "dbo.Project_Update",
                p => new
                    {
                        ProjectID = p.Int(),
                        ProjectName = p.String(maxLength: 50),
                        ProjectDescription = p.String(),
                        AssignedDeveloper = p.String(),
                        Budget = p.Decimal(precision: 19, scale: 4, storeType: "money"),
                        StartDate = p.DateTime(),
                        DeveloperID = p.Int(),
                        RowVersion_Original = p.Binary(maxLength: 8, fixedLength: true, storeType: "rowversion"),
                    },
                body:
                    @"UPDATE [dbo].[Project]
                      SET [ProjectName] = @ProjectName, [ProjectDescription] = @ProjectDescription, [AssignedDeveloper] = @AssignedDeveloper, [Budget] = @Budget, [StartDate] = @StartDate, [DeveloperID] = @DeveloperID
                      WHERE (([ProjectID] = @ProjectID) AND (([RowVersion] = @RowVersion_Original) OR ([RowVersion] IS NULL AND @RowVersion_Original IS NULL)))
                      
                      SELECT t0.[RowVersion]
                      FROM [dbo].[Project] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ProjectID] = @ProjectID"
            );
            
            AlterStoredProcedure(
                "dbo.Project_Delete",
                p => new
                    {
                        ProjectID = p.Int(),
                        RowVersion_Original = p.Binary(maxLength: 8, fixedLength: true, storeType: "rowversion"),
                    },
                body:
                    @"DELETE [dbo].[Project]
                      WHERE (([ProjectID] = @ProjectID) AND (([RowVersion] = @RowVersion_Original) OR ([RowVersion] IS NULL AND @RowVersion_Original IS NULL)))"
            );
            
        }
        
        public override void Down()
        {
            DropColumn("dbo.Project", "RowVersion");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
