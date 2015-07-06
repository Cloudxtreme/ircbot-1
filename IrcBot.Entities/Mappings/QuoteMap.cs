using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using IrcBot.Entities.Models;

namespace IrcBot.Entities.Mappings
{
    public sealed class QuoteMap : EntityTypeConfiguration<Quote>
    {
        public QuoteMap()
        {
            HasKey(x => x.Id);

            Property(x => x.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Author)
                .IsRequired()
                .HasMaxLength(64);

            Property(x => x.Content)
                .IsRequired();

            Property(x => x.Points)
                .IsRequired();

            Property(x => x.Created)
                .IsRequired();

            Property(x => x.Modified)
                .IsRequired();

            ToTable("Quotes");

            Property(x => x.Id).HasColumnName("Id");
            Property(x => x.Author).HasColumnName("Author");
            Property(x => x.Content).HasColumnName("Content");
            Property(x => x.Points).HasColumnName("Points");
            Property(x => x.Created).HasColumnName("Created");
            Property(x => x.Modified).HasColumnName("Modified");
        }
    }
}
