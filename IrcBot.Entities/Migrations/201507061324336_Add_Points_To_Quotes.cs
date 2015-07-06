namespace IrcBot.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Points_To_Quotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quotes", "Points", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Quotes", "Points");
        }
    }
}
