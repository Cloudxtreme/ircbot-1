namespace IrcBot.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Refactor_User_Model : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Email", c => c.String(nullable: false, maxLength: 64));
            AddColumn("dbo.Users", "Password", c => c.String(nullable: false));
            DropColumn("dbo.Users", "Nick");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Nick", c => c.String(nullable: false, maxLength: 64));
            DropColumn("dbo.Users", "Password");
            DropColumn("dbo.Users", "Email");
        }
    }
}
