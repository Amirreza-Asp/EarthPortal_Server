using Domain.Entities.Notices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.FluentConfig
{
    public class FluentNewsLinkConfig : IEntityTypeConfiguration<NewsLink>
    {

        public void Configure(EntityTypeBuilder<NewsLink> builder)
        {
            builder.HasKey(b => new { b.LinkId, b.NewsId });
        }
    }
}
