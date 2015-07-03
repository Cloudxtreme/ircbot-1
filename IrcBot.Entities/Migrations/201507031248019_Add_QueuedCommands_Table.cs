namespace IrcBot.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_QueuedCommands_Table : DbMigration
    {
        public override void Up()
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
        
        public override void Down()
        {
            DropTable("dbo.QueuedCommands");
        }
    }
}
