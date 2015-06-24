using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrcBot.Entities.Models;

namespace IrcBot.Entities.Mappings
{
    public sealed class PointMap : EntityTypeConfiguration<Point>
    {
        public PointMap()
        {
            HasKey(x => x.Id);

            Property(x => x.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Nick)
                .IsRequired()
                .HasMaxLength(64);

            Property(x => x.Value)
                .IsRequired();

            Property(x => x.Created)
                .IsRequired();

            Property(x => x.Modified)
                .IsRequired();

            ToTable("Points");

            Property(x => x.Id).HasColumnName("Id");
            Property(x => x.Nick).HasColumnName("Nick");
            Property(x => x.Value).HasColumnName("Value");
            Property(x => x.Created).HasColumnName("Created");
            Property(x => x.Modified).HasColumnName("Modified");
        }
    }
}
