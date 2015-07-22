namespace IrcBot.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_Points_From_Quotes : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Quotes", "Points");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Quotes", "Points", c => c.Int(nullable: false));
        }
    }
}
