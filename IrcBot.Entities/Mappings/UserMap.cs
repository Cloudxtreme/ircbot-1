using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using IrcBot.Entities.Models;

namespace IrcBot.Entities.Mappings
{
    public sealed class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            HasKey(x => x.Id);

            Property(x => x.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Nick)
                .IsRequired()
                .HasMaxLength(64);

            Property(x => x.Created)
                .IsRequired();

            Property(x => x.Modified)
                .IsRequired();

            ToTable("Users");

            Property(x => x.Id).HasColumnName("Id");
            Property(x => x.Nick).HasColumnName("Nick");
            Property(x => x.Created).HasColumnName("Created");
            Property(x => x.Modified).HasColumnName("Modified");
        }
    }
}
