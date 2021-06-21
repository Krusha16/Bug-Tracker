namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddContentToNotifications : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketNotifications", "Content", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicketNotifications", "Content");
        }
    }
}
