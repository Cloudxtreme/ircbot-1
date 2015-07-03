using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using IrcBot.Entities.Models;

namespace IrcBot.Entities.Mappings
{
    public sealed class QueuedCommandMap : EntityTypeConfiguration<QueuedCommand>
    {
        public QueuedCommandMap()
        {
            HasKey(x => x.Id);

            Property(x => x.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Command)
                .HasMaxLength(64);

            Property(x => x.Modified)
                .IsRequired();

            ToTable("QueuedCommands");

            Property(x => x.Id).HasColumnName("Id");
            Property(x => x.Command).HasColumnName("Command");
            Property(x => x.Created).HasColumnName("Created");
            Property(x => x.Modified).HasColumnName("Modified");
        }
    }
}
