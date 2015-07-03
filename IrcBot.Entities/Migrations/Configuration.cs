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

            var now = DateTime.Now;

            context.Users.AddOrUpdate(new []
            {
                new User
                {
                    Id = 1,
                    Email = "adamstirtan@gmail.com",
                    Password = PasswordEncryption.Encrypt("Super3vilGenius"),
                    Created = now,
                    Modified = now,
                    ObjectState = ObjectState.Added
                },
                new User
                {
                    Id = 2,
                    Email = "michaeljohndukich@gmail.com",
                    Password = PasswordEncryption.Encrypt("smellslikefarts"),
                    Created = now,
                    Modified = now,
                    ObjectState = ObjectState.Added
                },
                new User
                {
                    Id = 3,
                    Email = "donnycosby@gmail.com",
                    Password = PasswordEncryption.Encrypt("gameboy"),
                    Created = now,
                    Modified = now,
                    ObjectState = ObjectState.Added
                },
                new User
                {
                    Id = 4,
                    Email = "davef@dataforge.on.ca",
                    Password = PasswordEncryption.Encrypt("biglongdong"),
                    Created = now,
                    Modified = now,
                    ObjectState = ObjectState.Added
                },
                new User
                {
                    Id = 4,
                    Email = "kenbayton@gmail.com",
                    Password = PasswordEncryption.Encrypt("boyonfire"),
                    Created = now,
                    Modified = now,
                    ObjectState = ObjectState.Added
                }
            });
        }
    }
}
