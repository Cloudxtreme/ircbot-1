namespace IrcBot.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Points_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Points",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nick = c.String(nullable: false, maxLength: 64),
                        Value = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Points");
        }
    }
}
