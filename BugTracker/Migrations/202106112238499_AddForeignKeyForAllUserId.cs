namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKeyForAllUserId : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProjectUsers", new[] { "User_Id" });
            DropIndex("dbo.TicketAttachments", new[] { "User_Id" });
            DropIndex("dbo.Tickets", new[] { "AssignedToUser_Id" });
            DropIndex("dbo.Tickets", new[] { "OwnerUser_Id" });
            DropIndex("dbo.TicketComments", new[] { "User_Id" });
            DropIndex("dbo.TicketHistories", new[] { "User_Id" });
            DropIndex("dbo.TicketNotifications", new[] { "User_Id" });
            DropColumn("dbo.ProjectUsers", "UserId");
            DropColumn("dbo.TicketAttachments", "UserId");
            DropColumn("dbo.Tickets", "AssignedToUserId");
            DropColumn("dbo.Tickets", "OwnerUserId");
            DropColumn("dbo.TicketComments", "UserId");
            DropColumn("dbo.TicketHistories", "UserId");
            DropColumn("dbo.TicketNotifications", "UserId");
            RenameColumn(table: "dbo.ProjectUsers", name: "User_Id", newName: "UserId");
            RenameColumn(table: "dbo.TicketAttachments", name: "User_Id", newName: "UserId");
            RenameColumn(table: "dbo.TicketComments", name: "User_Id", newName: "UserId");
            RenameColumn(table: "dbo.TicketHistories", name: "User_Id", newName: "UserId");
            RenameColumn(table: "dbo.TicketNotifications", name: "User_Id", newName: "UserId");
            RenameColumn(table: "dbo.Tickets", name: "AssignedToUser_Id", newName: "AssignedToUserId");
            RenameColumn(table: "dbo.Tickets", name: "OwnerUser_Id", newName: "OwnerUserId");
            AlterColumn("dbo.ProjectUsers", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.TicketAttachments", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Tickets", "OwnerUserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Tickets", "AssignedToUserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.TicketComments", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.TicketHistories", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.TicketNotifications", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.ProjectUsers", "UserId");
            CreateIndex("dbo.TicketAttachments", "UserId");
            CreateIndex("dbo.Tickets", "OwnerUserId");
            CreateIndex("dbo.Tickets", "AssignedToUserId");
            CreateIndex("dbo.TicketComments", "UserId");
            CreateIndex("dbo.TicketHistories", "UserId");
            CreateIndex("dbo.TicketNotifications", "UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TicketNotifications", new[] { "UserId" });
            DropIndex("dbo.TicketHistories", new[] { "UserId" });
            DropIndex("dbo.TicketComments", new[] { "UserId" });
            DropIndex("dbo.Tickets", new[] { "AssignedToUserId" });
            DropIndex("dbo.Tickets", new[] { "OwnerUserId" });
            DropIndex("dbo.TicketAttachments", new[] { "UserId" });
            DropIndex("dbo.ProjectUsers", new[] { "UserId" });
            AlterColumn("dbo.TicketNotifications", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.TicketHistories", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.TicketComments", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.Tickets", "AssignedToUserId", c => c.Int());
            AlterColumn("dbo.Tickets", "OwnerUserId", c => c.Int(nullable: false));
            AlterColumn("dbo.TicketAttachments", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.ProjectUsers", "UserId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Tickets", name: "OwnerUserId", newName: "OwnerUser_Id");
            RenameColumn(table: "dbo.Tickets", name: "AssignedToUserId", newName: "AssignedToUser_Id");
            RenameColumn(table: "dbo.TicketNotifications", name: "UserId", newName: "User_Id");
            RenameColumn(table: "dbo.TicketHistories", name: "UserId", newName: "User_Id");
            RenameColumn(table: "dbo.TicketComments", name: "UserId", newName: "User_Id");
            RenameColumn(table: "dbo.TicketAttachments", name: "UserId", newName: "User_Id");
            RenameColumn(table: "dbo.ProjectUsers", name: "UserId", newName: "User_Id");
            AddColumn("dbo.TicketNotifications", "UserId", c => c.Int(nullable: false));
            AddColumn("dbo.TicketHistories", "UserId", c => c.Int(nullable: false));
            AddColumn("dbo.TicketComments", "UserId", c => c.Int(nullable: false));
            AddColumn("dbo.Tickets", "OwnerUserId", c => c.Int(nullable: false));
            AddColumn("dbo.Tickets", "AssignedToUserId", c => c.Int());
            AddColumn("dbo.TicketAttachments", "UserId", c => c.Int(nullable: false));
            AddColumn("dbo.ProjectUsers", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.TicketNotifications", "User_Id");
            CreateIndex("dbo.TicketHistories", "User_Id");
            CreateIndex("dbo.TicketComments", "User_Id");
            CreateIndex("dbo.Tickets", "OwnerUser_Id");
            CreateIndex("dbo.Tickets", "AssignedToUser_Id");
            CreateIndex("dbo.TicketAttachments", "User_Id");
            CreateIndex("dbo.ProjectUsers", "User_Id");
        }
    }
}
