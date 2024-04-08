using Domain.Entities.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.FluentConfig.Pages
{
    public class FluentHomePageConfig : IEntityTypeConfiguration<HomePage>
    {
        public void Configure(EntityTypeBuilder<HomePage> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).ValueGeneratedNever();

            builder.OwnsOne(b => b.Header, d =>
            {
                d.Property(e => e.Content).HasColumnName("HeaderContent").IsRequired();
                d.Property(e => e.Title).HasColumnName("HeaderTitle").IsRequired();
                d.Property(e => e.PortBtnEnable).HasColumnName("HeaderPortBtnEnable").IsRequired();
                d.Property(e => e.AppBtnEnable).HasColumnName("HeaderAppBtnEnable").IsRequired();
            });

            builder.OwnsOne(b => b.Work, d =>
            {
                d.Property(e => e.Content).HasColumnName("WorkContent").IsRequired();
                d.Property(e => e.Title).HasColumnName("WorkTitle").IsRequired();
                d.Property(e => e.Port).HasColumnName("WorkPort").IsRequired();
                d.Property(e => e.App).HasColumnName("WorkApp").IsRequired();
            });
        }
    }
}
