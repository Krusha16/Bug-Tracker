namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAllClassesWithProperties : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProjectUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.ProjectId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.TicketAttachments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        Description = c.String(),
                        FileUrl = c.String(),
                        Created = c.DateTime(nullable: false),
                        TicketId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.TicketId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        ProjectId = c.Int(nullable: false),
                        OwnerUserId = c.Int(nullable: false),
                        AssignedToUserId = c.Int(),
                        TicketTypeId = c.Int(nullable: false),
                        TicketPriorityId = c.Int(nullable: false),
                        TicketStatusId = c.Int(nullable: false),
                        AssignedToUser_Id = c.String(maxLength: 128),
                        OwnerUser_Id = c.String(maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AssignedToUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerUser_Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.TicketPriorities", t => t.TicketPriorityId, cascadeDelete: true)
                .ForeignKey("dbo.TicketStatus", t => t.TicketStatusId, cascadeDelete: true)
                .ForeignKey("dbo.TicketTypes", t => t.TicketTypeId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ProjectId)
                .Index(t => t.TicketTypeId)
                .Index(t => t.TicketPriorityId)
                .Index(t => t.TicketStatusId)
                .Index(t => t.AssignedToUser_Id)
                .Index(t => t.OwnerUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.TicketComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Comment = c.String(),
                        Created = c.DateTime(nullable: false),
                        TicketId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.TicketId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.TicketHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Property = c.String(),
                        OldValue = c.String(),
                        NewValue = c.String(),
                        Changed = c.String(),
                        TicketId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.TicketId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.TicketNotifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TicketId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.TicketId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.TicketPriorities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TicketStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TicketTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TicketAttachments", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tickets", "TicketTypeId", "dbo.TicketTypes");
            DropForeignKey("dbo.Tickets", "TicketStatusId", "dbo.TicketStatus");
            DropForeignKey("dbo.Tickets", "TicketPriorityId", "dbo.TicketPriorities");
            DropForeignKey("dbo.TicketNotifications", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TicketNotifications", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.TicketHistories", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TicketHistories", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.TicketComments", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TicketComments", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.TicketAttachments", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.Tickets", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Tickets", "OwnerUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tickets", "AssignedToUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProjectUsers", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProjectUsers", "ProjectId", "dbo.Projects");
            DropIndex("dbo.TicketNotifications", new[] { "User_Id" });
            DropIndex("dbo.TicketNotifications", new[] { "TicketId" });
            DropIndex("dbo.TicketHistories", new[] { "User_Id" });
            DropIndex("dbo.TicketHistories", new[] { "TicketId" });
            DropIndex("dbo.TicketComments", new[] { "User_Id" });
            DropIndex("dbo.TicketComments", new[] { "TicketId" });
            DropIndex("dbo.Tickets", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Tickets", new[] { "OwnerUser_Id" });
            DropIndex("dbo.Tickets", new[] { "AssignedToUser_Id" });
            DropIndex("dbo.Tickets", new[] { "TicketStatusId" });
            DropIndex("dbo.Tickets", new[] { "TicketPriorityId" });
            DropIndex("dbo.Tickets", new[] { "TicketTypeId" });
            DropIndex("dbo.Tickets", new[] { "ProjectId" });
            DropIndex("dbo.TicketAttachments", new[] { "User_Id" });
            DropIndex("dbo.TicketAttachments", new[] { "TicketId" });
            DropIndex("dbo.ProjectUsers", new[] { "User_Id" });
            DropIndex("dbo.ProjectUsers", new[] { "ProjectId" });
            DropTable("dbo.TicketTypes");
            DropTable("dbo.TicketStatus");
            DropTable("dbo.TicketPriorities");
            DropTable("dbo.TicketNotifications");
            DropTable("dbo.TicketHistories");
            DropTable("dbo.TicketComments");
            DropTable("dbo.Tickets");
            DropTable("dbo.TicketAttachments");
            DropTable("dbo.ProjectUsers");
            DropTable("dbo.Projects");
        }
    }
}
