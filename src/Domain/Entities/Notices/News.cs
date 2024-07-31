using Domain.Shared;

namespace Domain.Entities.Notices
{
    public class News : BaseEntity
    {
        public News(string title, string description, string headline, string source, DateTime dateOfRegisration, Guid newsCategoryId, int shortLink) : base(Guid.NewGuid())
        {
            Title = title;
            Description = description;
            Headline = headline;
            Source = source;
            DateOfRegisration = dateOfRegisration;
            NewsCategoryId = newsCategoryId;
            ShortLink = shortLink;
        }

        private News() { }

        public String Title { get; set; }
        public String Description { get; set; }
        public String Headline { get; set; }
        public String Source { get; set; }
        public DateTime DateOfRegisration { get; set; }
        public int ShortLink { get; set; }
        public long Seen { get; set; }

        public Guid NewsCategoryId { get; set; }

        public NewsCategory? NewsCategory { get; set; }
        public ICollection<NewsLink> Links { get; set; }
        public ICollection<NewsImage> Images { get; set; }
    }
}
