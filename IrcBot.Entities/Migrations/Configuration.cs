using System.Data.Entity.Migrations;

namespace IrcBot.Entities.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<IrcBotContext>
    {
        private const bool DebugEnabled = false;

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(IrcBotContext context)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (DebugEnabled)
#pragma warning disable 162
            {
                System.Diagnostics.Debugger.Launch();
            }
#pragma warning restore 162
        }
    }
}
