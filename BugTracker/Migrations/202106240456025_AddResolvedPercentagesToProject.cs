namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddResolvedPercentagesToProject : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "ResolvedPercentage", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "ResolvedPercentage");
        }
    }
}
