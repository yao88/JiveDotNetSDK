namespace Net.Pokeshot.JiveSdk.Example.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jivesdk2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WorkflowDefinitions", "JiveInstance_JiveInstanceId", "dbo.JiveInstances");
            DropForeignKey("dbo.Workflows", "JiveInstance_JiveInstanceId", "dbo.JiveInstances");
            DropForeignKey("dbo.JivePlaces", "JiveInstance_JiveInstanceId", "dbo.JiveInstances");
            DropForeignKey("dbo.Workflows", "JivePlace_JivePlaceId", "dbo.JivePlaces");
            DropForeignKey("dbo.Workflows", "WorkflowDefinition_WorkflowDefinitionId", "dbo.WorkflowDefinitions");
            DropForeignKey("dbo.JiveContents", "JiveInstance_JiveInstanceId", "dbo.JiveInstances");
            DropForeignKey("dbo.WorkflowInstances", "JiveContent_JiveContentId", "dbo.JiveContents");
            DropForeignKey("dbo.WorkflowInstances", "Workflow_WorkflowId", "dbo.Workflows");
            DropIndex("dbo.WorkflowDefinitions", new[] { "JiveInstance_JiveInstanceId" });
            DropIndex("dbo.Workflows", new[] { "JiveInstance_JiveInstanceId" });
            DropIndex("dbo.Workflows", new[] { "JivePlace_JivePlaceId" });
            DropIndex("dbo.Workflows", new[] { "WorkflowDefinition_WorkflowDefinitionId" });
            DropIndex("dbo.JivePlaces", new[] { "JiveInstance_JiveInstanceId" });
            DropIndex("dbo.WorkflowInstances", new[] { "JiveContent_JiveContentId" });
            DropIndex("dbo.WorkflowInstances", new[] { "Workflow_WorkflowId" });
            DropIndex("dbo.JiveContents", new[] { "JiveInstance_JiveInstanceId" });
            DropTable("dbo.WorkflowDefinitions");
            DropTable("dbo.Workflows");
            DropTable("dbo.JivePlaces");
            DropTable("dbo.WorkflowInstances");
            DropTable("dbo.JiveContents");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.JiveContents",
                c => new
                    {
                        JiveContentId = c.String(nullable: false, maxLength: 128),
                        JiveInstance_JiveInstanceId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.JiveContentId);
            
            CreateTable(
                "dbo.WorkflowInstances",
                c => new
                    {
                        WorkflowInstanceId = c.Int(nullable: false, identity: true),
                        DateCreated = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        JiveContent_JiveContentId = c.String(maxLength: 128),
                        Workflow_WorkflowId = c.Int(),
                    })
                .PrimaryKey(t => t.WorkflowInstanceId);
            
            CreateTable(
                "dbo.JivePlaces",
                c => new
                    {
                        JivePlaceId = c.String(nullable: false, maxLength: 128),
                        DateCreated = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        JiveInstance_JiveInstanceId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.JivePlaceId);
            
            CreateTable(
                "dbo.Workflows",
                c => new
                    {
                        WorkflowId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Status = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        JiveInstance_JiveInstanceId = c.String(maxLength: 128),
                        JivePlace_JivePlaceId = c.String(maxLength: 128),
                        WorkflowDefinition_WorkflowDefinitionId = c.Int(),
                    })
                .PrimaryKey(t => t.WorkflowId);
            
            CreateTable(
                "dbo.WorkflowDefinitions",
                c => new
                    {
                        WorkflowDefinitionId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        JiveInstance_JiveInstanceId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.WorkflowDefinitionId);
            
            CreateIndex("dbo.JiveContents", "JiveInstance_JiveInstanceId");
            CreateIndex("dbo.WorkflowInstances", "Workflow_WorkflowId");
            CreateIndex("dbo.WorkflowInstances", "JiveContent_JiveContentId");
            CreateIndex("dbo.JivePlaces", "JiveInstance_JiveInstanceId");
            CreateIndex("dbo.Workflows", "WorkflowDefinition_WorkflowDefinitionId");
            CreateIndex("dbo.Workflows", "JivePlace_JivePlaceId");
            CreateIndex("dbo.Workflows", "JiveInstance_JiveInstanceId");
            CreateIndex("dbo.WorkflowDefinitions", "JiveInstance_JiveInstanceId");
            AddForeignKey("dbo.WorkflowInstances", "Workflow_WorkflowId", "dbo.Workflows", "WorkflowId");
            AddForeignKey("dbo.WorkflowInstances", "JiveContent_JiveContentId", "dbo.JiveContents", "JiveContentId");
            AddForeignKey("dbo.JiveContents", "JiveInstance_JiveInstanceId", "dbo.JiveInstances", "JiveInstanceId");
            AddForeignKey("dbo.Workflows", "WorkflowDefinition_WorkflowDefinitionId", "dbo.WorkflowDefinitions", "WorkflowDefinitionId");
            AddForeignKey("dbo.Workflows", "JivePlace_JivePlaceId", "dbo.JivePlaces", "JivePlaceId");
            AddForeignKey("dbo.JivePlaces", "JiveInstance_JiveInstanceId", "dbo.JiveInstances", "JiveInstanceId");
            AddForeignKey("dbo.Workflows", "JiveInstance_JiveInstanceId", "dbo.JiveInstances", "JiveInstanceId");
            AddForeignKey("dbo.WorkflowDefinitions", "JiveInstance_JiveInstanceId", "dbo.JiveInstances", "JiveInstanceId");
        }
    }
}
