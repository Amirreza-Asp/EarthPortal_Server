using Domain.Entities.Regulation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.FluentConfig;

public class FluentLawConfig : IEntityTypeConfiguration<Law>
{
    public void Configure(EntityTypeBuilder<Law> builder)
    {
        builder.OwnsOne(
            b => b.Announcement,
            b =>
            {
                b.Property(e => e.Number).HasColumnName("AnnouncementNumber");
                b.Property(e => e.Date).HasColumnName("AnnouncementDate");
            }
        );

        builder.OwnsOne(
            b => b.Newspaper,
            b =>
            {
                b.Property(e => e.Number).HasColumnName("NewspaperNumber");
                b.Property(e => e.Date).HasColumnName("NewspaperDate");
                b.Property(e => e.File).HasColumnName("NewspaperFile").IsRequired(false);
            }
        );
    }
}
