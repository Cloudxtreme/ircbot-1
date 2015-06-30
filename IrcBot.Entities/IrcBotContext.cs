using System.Data.Entity;

using IrcBot.Database.Entity;
using IrcBot.Entities.Mappings;
using IrcBot.Entities.Models;

namespace IrcBot.Entities
{
    public class IrcBotContext : DataContext
    {
        static IrcBotContext()
        {
            System.Data.Entity.Database.SetInitializer<IrcBotContext>(null);
        }

        public IrcBotContext()
            : base("name=IrcBotContext")
        { }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new MessageMap());
            modelBuilder.Configurations.Add(new PointMap());
            modelBuilder.Configurations.Add(new QuoteMap());
            modelBuilder.Configurations.Add(new UserMap());
        }
    }
}
