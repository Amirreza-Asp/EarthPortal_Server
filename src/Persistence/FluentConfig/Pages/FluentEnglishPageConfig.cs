using Domain.Entities.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.FluentConfig.Pages
{
    public class FluentEnglishPageConfig : IEntityTypeConfiguration<EnglishPage>
    {
        public void Configure(EntityTypeBuilder<EnglishPage> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(b => b.Id).ValueGeneratedNever();

            builder.OwnsOne(b => b.Intro, d =>
            {
                d.Property(e => e.Title).HasColumnName("IntroTitle").IsRequired();
                d.Property(e => e.Content).HasColumnName("IntroContent").IsRequired();
            });

            builder.OwnsOne(b => b.MainIdea, d =>
            {
                d.Property(e => e.Title).HasColumnName("MainIdeaTitle").IsRequired();
                d.Property(e => e.Bold).HasColumnName("MainIdeaBold").IsRequired();
                d.Property(e => e.Content1).HasColumnName("MainIdeaContent1").IsRequired();
                d.Property(e => e.Content2).HasColumnName("MainIdeaContent2").IsRequired();
            });

            builder.OwnsOne(b => b.CurrentSituation, d =>
            {
                d.Property(e => e.Title).HasColumnName("CurrentSituationTitle").IsRequired();
                d.Property(e => e.Content).HasColumnName("CurrentSituationContent").IsRequired();
                d.Property(e => e.Image).HasColumnName("CurrentSituationImage").IsRequired();
            });

            builder.OwnsOne(b => b.Vision, d =>
            {
                d.Property(e => e.Title).HasColumnName("VisionTitle").IsRequired();
                d.Property(e => e.Content).HasColumnName("VisionContent").IsRequired();
            });


            builder.HasMany(b => b.Cards)
                .WithOne(b => b.EnglishPage)
                .HasForeignKey(b => b.EnglishPageId);
        }
    }
}
