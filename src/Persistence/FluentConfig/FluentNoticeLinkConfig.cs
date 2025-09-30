using Domain.Entities.Notices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.FluentConfig
{
    public class FluentNoticeLinkConfig : IEntityTypeConfiguration<NoticeLink>
    {

        public void Configure(EntityTypeBuilder<NoticeLink> builder)
        {
            builder.HasKey(b => new { b.LinkId, b.NoticeId });
        }
    }
}
