using Domain.Dtos.Resources;
using Domain.Entities.Account;
using Domain.Entities.Contact;
using Domain.Entities.Mutimedia;
using Domain.Entities.Notices;
using Domain.Entities.Pages;
using Domain.Entities.Regulation;
using Domain.Entities.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace Persistence
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IMemoryCache _memoryCache;
        public ApplicationDbContext(DbContextOptions options, IMemoryCache memoryCache) : base(options)
        {
            _memoryCache = memoryCache;
        }

        #region Account
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        #endregion

        #region Regulation
        public DbSet<Law> Law { get; set; }
        public DbSet<ApprovalAuthority> ApprovalAuthority { get; set; }
        public DbSet<ApprovalStatus> ApprovalStatus { get; set; }
        public DbSet<ApprovalType> ApprovalType { get; set; }
        public DbSet<ExecutorManagment> ExecutorManagment { get; set; }
        public DbSet<LawCategory> LawCategory { get; set; }
        public DbSet<LawImage> LawImage { get; set; }
        #endregion

        #region Multimedia
        public DbSet<Gallery> Gallery { get; set; }
        public DbSet<GalleryPhoto> GalleryPhoto { get; set; }
        public DbSet<VideoContent> VideoContent { get; set; }
        public DbSet<Infographic> Infographic { get; set; }
        #endregion

        #region Notices
        public DbSet<NewsCategory> NewsCategory { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<NewsImage> NewsImage { get; set; }
        public DbSet<Link> Link { get; set; }
        public DbSet<NewsLink> NewsLink { get; set; }
        #endregion

        #region Contact
        public DbSet<FrequentlyAskedQuestions> FrequentlyAskedQuestions { get; set; }

        public DbSet<Guide> Guide { get; set; }

        public DbSet<Info> Info { get; set; }
        public DbSet<GeoAddress> GeoAddress { get; set; }
        public DbSet<EducationalVideo> EducationalVideo { get; set; }
        public DbSet<RelatedCompany> RelatedCompany { get; set; }
        public DbSet<Goal> Goal { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }

        public DbSet<RelatedLink> RelatedLink { get; set; }

        public DbSet<SystemEvaluation> SystemEvaluation { get; set; }
        public DbSet<SystemEvaluationIntroductionMethod> IntroductionMethod { get; set; }
        public DbSet<SystemEvaluationPage> SystemEvaluationPage { get; set; }
        public DbSet<Statistics> Statistics { get; set; }
        #endregion

        #region Resources
        public DbSet<Author> Author { get; set; }
        public DbSet<Translator> Translator { get; set; }
        public DbSet<Publication> Publication { get; set; }

        public DbSet<Book> Book { get; set; }
        public DbSet<Broadcast> Broadcast { get; set; }
        public DbSet<Article> Article { get; set; }

        #endregion

        #region Pages
        public DbSet<HomePage> HomePage { get; set; }
        public DbSet<AboutUsPage> AboutUsPage { get; set; }
        public DbSet<LawPage> LawPage { get; set; }
        public DbSet<EnglishPage> EnglishPage { get; set; }
        public DbSet<EnglishProblem> EnglishPageProblem { get; set; }
        public DbSet<EnglishSolution> EnglishPageSolution { get; set; }
        public DbSet<EnglishCard> EnglishCard { get; set; }
        public DbSet<FooterPage> FooterPage { get; set; }

        public DbSet<PageMetadata> PageMetadata { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<News>().Property(b => b.Id).ValueGeneratedNever();
            modelBuilder.Entity<Link>().Property(b => b.Id).ValueGeneratedNever();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var footer = await this.FooterPage.FirstOrDefaultAsync(cancellationToken);

            if (footer == null)
                this.FooterPage.Add(new FooterPage(Guid.NewGuid()));
            else
            {
                footer.LastUpdate = DateTime.Now.AddHours(-1);
                this.FooterPage.Update(footer);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}
