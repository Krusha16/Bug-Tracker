namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTypeOfChangedInHistory : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TicketHistories", "Changed", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TicketHistories", "Changed", c => c.String());
        }
    }
}
