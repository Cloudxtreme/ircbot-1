namespace IrcBot.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_Queued_Command_Table : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.QueuedCommands");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.QueuedCommands",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Command = c.String(maxLength: 64),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
