using Domain.Entities.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.FluentConfig.Pages
{
    public class FluentPageMetadataConfig : IEntityTypeConfiguration<PageMetadata>
    {
        public void Configure(EntityTypeBuilder<PageMetadata> builder)
        {
            builder.ToTable("PageMetadata");

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).ValueGeneratedNever();

            builder.Property(b => b.Title).IsRequired();
            builder.Property(b => b.Description).IsRequired();

            builder.Property(b => b.Page).IsRequired();
            builder.HasIndex(b => b.Page).IsUnique();

            builder.OwnsMany(b => b.Keywords, e =>
            {
                e.ToTable("PageMetadataKeywords");

                e.Property(b => b.Value).HasColumnName("Value");
            });
        }
    }
}
