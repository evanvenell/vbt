namespace BugTrackerApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectSP : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(
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
                      
                      SELECT t0.[ProjectID]
                      FROM [dbo].[Project] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ProjectID] = @ProjectID"
            );
            
            CreateStoredProcedure(
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
                    },
                body:
                    @"UPDATE [dbo].[Project]
                      SET [ProjectName] = @ProjectName, [ProjectDescription] = @ProjectDescription, [AssignedDeveloper] = @AssignedDeveloper, [Budget] = @Budget, [StartDate] = @StartDate, [DeveloperID] = @DeveloperID
                      WHERE ([ProjectID] = @ProjectID)"
            );
            
            CreateStoredProcedure(
                "dbo.Project_Delete",
                p => new
                    {
                        ProjectID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Project]
                      WHERE ([ProjectID] = @ProjectID)"
            );
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.Project_Delete");
            DropStoredProcedure("dbo.Project_Update");
            DropStoredProcedure("dbo.Project_Insert");
        }
    }
}
