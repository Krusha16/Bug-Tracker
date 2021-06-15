namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeFewTicketPropertiesNullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tickets", "TicketPriorityId", "dbo.TicketPriorities");
            DropForeignKey("dbo.Tickets", "TicketStatusId", "dbo.TicketStatus");
            DropForeignKey("dbo.Tickets", "TicketTypeId", "dbo.TicketTypes");
            DropIndex("dbo.Tickets", new[] { "TicketTypeId" });
            DropIndex("dbo.Tickets", new[] { "TicketPriorityId" });
            DropIndex("dbo.Tickets", new[] { "TicketStatusId" });
            AlterColumn("dbo.Tickets", "Updated", c => c.DateTime());
            AlterColumn("dbo.Tickets", "TicketTypeId", c => c.Int());
            AlterColumn("dbo.Tickets", "TicketPriorityId", c => c.Int());
            AlterColumn("dbo.Tickets", "TicketStatusId", c => c.Int());
            CreateIndex("dbo.Tickets", "TicketTypeId");
            CreateIndex("dbo.Tickets", "TicketPriorityId");
            CreateIndex("dbo.Tickets", "TicketStatusId");
            AddForeignKey("dbo.Tickets", "TicketPriorityId", "dbo.TicketPriorities", "Id");
            AddForeignKey("dbo.Tickets", "TicketStatusId", "dbo.TicketStatus", "Id");
            AddForeignKey("dbo.Tickets", "TicketTypeId", "dbo.TicketTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "TicketTypeId", "dbo.TicketTypes");
            DropForeignKey("dbo.Tickets", "TicketStatusId", "dbo.TicketStatus");
            DropForeignKey("dbo.Tickets", "TicketPriorityId", "dbo.TicketPriorities");
            DropIndex("dbo.Tickets", new[] { "TicketStatusId" });
            DropIndex("dbo.Tickets", new[] { "TicketPriorityId" });
            DropIndex("dbo.Tickets", new[] { "TicketTypeId" });
            AlterColumn("dbo.Tickets", "TicketStatusId", c => c.Int(nullable: false));
            AlterColumn("dbo.Tickets", "TicketPriorityId", c => c.Int(nullable: false));
            AlterColumn("dbo.Tickets", "TicketTypeId", c => c.Int(nullable: false));
            AlterColumn("dbo.Tickets", "Updated", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Tickets", "TicketStatusId");
            CreateIndex("dbo.Tickets", "TicketPriorityId");
            CreateIndex("dbo.Tickets", "TicketTypeId");
            AddForeignKey("dbo.Tickets", "TicketTypeId", "dbo.TicketTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Tickets", "TicketStatusId", "dbo.TicketStatus", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Tickets", "TicketPriorityId", "dbo.TicketPriorities", "Id", cascadeDelete: true);
        }
    }
}
