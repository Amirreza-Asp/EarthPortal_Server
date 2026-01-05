using Domain.Entities.Regulation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.FluentConfig;

public class FluentLawLawContentConfig : IEntityTypeConfiguration<LawLawContent>
{
    public void Configure(EntityTypeBuilder<LawLawContent> builder)
    {
        builder.HasKey(x => new { x.LawId, x.LawContentId });

        builder.HasOne(x => x.Law).WithMany(x => x.LawLawContents).HasForeignKey(x => x.LawId);
        builder
            .HasOne(x => x.LawContent)
            .WithMany(x => x.LawLawContents)
            .HasForeignKey(x => x.LawContentId);

        builder.ToTable("LawLawContents");
    }
}
