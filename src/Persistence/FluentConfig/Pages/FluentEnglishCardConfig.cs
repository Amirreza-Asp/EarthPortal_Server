using Domain.Entities.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.FluentConfig.Pages
{
    public class FluentEnglishCardConfig : IEntityTypeConfiguration<EnglishCard>
    {

        public void Configure(EntityTypeBuilder<EnglishCard> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).ValueGeneratedNever();

        }
    }
}
