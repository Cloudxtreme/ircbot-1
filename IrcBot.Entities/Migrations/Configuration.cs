using System;
using System.Data.Entity.Migrations;
using IrcBot.Common.Encryption;
using IrcBot.Database.Infrastructure;
using IrcBot.Entities.Models;

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

            var utcNow = DateTime.UtcNow;

            context.Users.AddOrUpdate(new []
            {
                new User
                {
                    Email = "adamstirtan@gmail.com",
                    Password = PasswordEncryption.Encrypt("Super3vilGenius"),
                    Created = utcNow,
                    Modified = utcNow,
                    ObjectState = ObjectState.Added
                },
                new User
                {
                    Email = "michaeljohndukich@gmail.com",
                    Password = PasswordEncryption.Encrypt("smellslikefarts"),
                    Created = utcNow,
                    Modified = utcNow,
                    ObjectState = ObjectState.Added
                }
            });
        }
    }
}
