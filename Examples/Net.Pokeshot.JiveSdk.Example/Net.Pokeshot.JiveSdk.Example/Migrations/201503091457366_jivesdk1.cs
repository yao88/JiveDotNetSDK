namespace Net.Pokeshot.JiveSdk.Example.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jivesdk1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JiveAddons",
                c => new
                    {
                        JiveAddonId = c.Int(nullable: false, identity: true),
                        AddonType = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        ClientId = c.String(),
                        ClientSecret = c.String(),
                        JiveSignatureURL = c.String(),
                        TimeStamp = c.String(),
                        JiveSignature = c.String(),
                        Scope = c.String(),
                        Code = c.String(),
                        Uninstalled = c.Boolean(nullable: false),
                        JiveInstance_JiveInstanceId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.JiveAddonId)
                .ForeignKey("dbo.JiveInstances", t => t.JiveInstance_JiveInstanceId)
                .Index(t => t.JiveInstance_JiveInstanceId);
            
            CreateTable(
                "dbo.JiveInstances",
                c => new
                    {
                        JiveInstanceId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Comment = c.String(),
                        Url = c.String(),
                        CommunityLanguage = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        IsComplete = c.Boolean(nullable: false),
                        Version = c.String(),
                        IsLicensed = c.Boolean(nullable: false),
                        IsInstalledViaAddon = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.JiveInstanceId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        DisplayName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        EmailAddress = c.String(),
                        Department = c.String(),
                        IsComplete = c.Boolean(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        HasInstalledApp = c.Boolean(nullable: false),
                        RefreshToken = c.String(),
                        AccessToken = c.String(),
                        JiveInstance_JiveInstanceId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.JiveInstances", t => t.JiveInstance_JiveInstanceId)
                .Index(t => t.JiveInstance_JiveInstanceId);
            
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
                .PrimaryKey(t => t.WorkflowDefinitionId)
                .ForeignKey("dbo.JiveInstances", t => t.JiveInstance_JiveInstanceId)
                .Index(t => t.JiveInstance_JiveInstanceId);
            
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
                .PrimaryKey(t => t.WorkflowId)
                .ForeignKey("dbo.JiveInstances", t => t.JiveInstance_JiveInstanceId)
                .ForeignKey("dbo.JivePlaces", t => t.JivePlace_JivePlaceId)
                .ForeignKey("dbo.WorkflowDefinitions", t => t.WorkflowDefinition_WorkflowDefinitionId)
                .Index(t => t.JiveInstance_JiveInstanceId)
                .Index(t => t.JivePlace_JivePlaceId)
                .Index(t => t.WorkflowDefinition_WorkflowDefinitionId);
            
            CreateTable(
                "dbo.JivePlaces",
                c => new
                    {
                        JivePlaceId = c.String(nullable: false, maxLength: 128),
                        DateCreated = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        JiveInstance_JiveInstanceId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.JivePlaceId)
                .ForeignKey("dbo.JiveInstances", t => t.JiveInstance_JiveInstanceId)
                .Index(t => t.JiveInstance_JiveInstanceId);
            
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
                .PrimaryKey(t => t.WorkflowInstanceId)
                .ForeignKey("dbo.JiveContents", t => t.JiveContent_JiveContentId)
                .ForeignKey("dbo.Workflows", t => t.Workflow_WorkflowId)
                .Index(t => t.JiveContent_JiveContentId)
                .Index(t => t.Workflow_WorkflowId);
            
            CreateTable(
                "dbo.JiveContents",
                c => new
                    {
                        JiveContentId = c.String(nullable: false, maxLength: 128),
                        JiveInstance_JiveInstanceId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.JiveContentId)
                .ForeignKey("dbo.JiveInstances", t => t.JiveInstance_JiveInstanceId)
                .Index(t => t.JiveInstance_JiveInstanceId);
            
            CreateTable(
                "dbo.JiveTileInstances",
                c => new
                    {
                        JiveTileInstanceId = c.Int(nullable: false, identity: true),
                        TileId = c.Int(nullable: false),
                        AccessToken = c.String(),
                        RefreshToken = c.String(),
                        ExpiresIn = c.Int(nullable: false),
                        Url = c.String(),
                        TileType = c.String(),
                        JiveAddon_JiveAddonId = c.Int(),
                    })
                .PrimaryKey(t => t.JiveTileInstanceId)
                .ForeignKey("dbo.JiveAddons", t => t.JiveAddon_JiveAddonId)
                .Index(t => t.JiveAddon_JiveAddonId);
            
            CreateTable(
                "dbo.JiveAddonRegistrations",
                c => new
                    {
                        JiveAddonRegistrationId = c.Int(nullable: false, identity: true),
                        Timestamp = c.String(),
                        JiveSignatureURL = c.String(),
                        TenantId = c.String(),
                        JiveUrl = c.String(),
                        JiveSignature = c.String(),
                        ClientSecret = c.String(),
                        ClientId = c.String(),
                        Uninstalled = c.String(),
                        Code = c.String(),
                        Scope = c.String(),
                    })
                .PrimaryKey(t => t.JiveAddonRegistrationId);
            
            CreateTable(
                "dbo.JiveTileRegistrations",
                c => new
                    {
                        JiveTileRegistrationId = c.Int(nullable: false, identity: true),
                        Guid = c.String(),
                        Id = c.String(),
                        Config = c.String(),
                        Name = c.String(),
                        JiveUrl = c.String(),
                        TenantId = c.String(),
                        Url = c.String(),
                        Parent = c.String(),
                        PlaceUri = c.String(),
                        Code = c.String(),
                    })
                .PrimaryKey(t => t.JiveTileRegistrationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JiveTileInstances", "JiveAddon_JiveAddonId", "dbo.JiveAddons");
            DropForeignKey("dbo.WorkflowInstances", "Workflow_WorkflowId", "dbo.Workflows");
            DropForeignKey("dbo.WorkflowInstances", "JiveContent_JiveContentId", "dbo.JiveContents");
            DropForeignKey("dbo.JiveContents", "JiveInstance_JiveInstanceId", "dbo.JiveInstances");
            DropForeignKey("dbo.Workflows", "WorkflowDefinition_WorkflowDefinitionId", "dbo.WorkflowDefinitions");
            DropForeignKey("dbo.Workflows", "JivePlace_JivePlaceId", "dbo.JivePlaces");
            DropForeignKey("dbo.JivePlaces", "JiveInstance_JiveInstanceId", "dbo.JiveInstances");
            DropForeignKey("dbo.Workflows", "JiveInstance_JiveInstanceId", "dbo.JiveInstances");
            DropForeignKey("dbo.WorkflowDefinitions", "JiveInstance_JiveInstanceId", "dbo.JiveInstances");
            DropForeignKey("dbo.Users", "JiveInstance_JiveInstanceId", "dbo.JiveInstances");
            DropForeignKey("dbo.JiveAddons", "JiveInstance_JiveInstanceId", "dbo.JiveInstances");
            DropIndex("dbo.JiveTileInstances", new[] { "JiveAddon_JiveAddonId" });
            DropIndex("dbo.JiveContents", new[] { "JiveInstance_JiveInstanceId" });
            DropIndex("dbo.WorkflowInstances", new[] { "Workflow_WorkflowId" });
            DropIndex("dbo.WorkflowInstances", new[] { "JiveContent_JiveContentId" });
            DropIndex("dbo.JivePlaces", new[] { "JiveInstance_JiveInstanceId" });
            DropIndex("dbo.Workflows", new[] { "WorkflowDefinition_WorkflowDefinitionId" });
            DropIndex("dbo.Workflows", new[] { "JivePlace_JivePlaceId" });
            DropIndex("dbo.Workflows", new[] { "JiveInstance_JiveInstanceId" });
            DropIndex("dbo.WorkflowDefinitions", new[] { "JiveInstance_JiveInstanceId" });
            DropIndex("dbo.Users", new[] { "JiveInstance_JiveInstanceId" });
            DropIndex("dbo.JiveAddons", new[] { "JiveInstance_JiveInstanceId" });
            DropTable("dbo.JiveTileRegistrations");
            DropTable("dbo.JiveAddonRegistrations");
            DropTable("dbo.JiveTileInstances");
            DropTable("dbo.JiveContents");
            DropTable("dbo.WorkflowInstances");
            DropTable("dbo.JivePlaces");
            DropTable("dbo.Workflows");
            DropTable("dbo.WorkflowDefinitions");
            DropTable("dbo.Users");
            DropTable("dbo.JiveInstances");
            DropTable("dbo.JiveAddons");
        }
    }
}
