using Domain.Shared;

namespace Domain.Entities.Notices
{
    public class News : BaseEntity
    {
        public News(string title, string shortDescription, string description, string headline, string source, string shortLink, DateTime dateOfRegisration, Guid newsCategoryId)
        {
            Title = title;
            ShortDescription = shortDescription;
            Description = description;
            Headline = headline;
            Source = source;
            ShortLink = shortLink;
            DateOfRegisration = dateOfRegisration;
            NewsCategoryId = newsCategoryId;
        }

        private News() { }

        public String Title { get; set; }
        public String ShortDescription { get; set; }
        public String Description { get; set; }
        public String Headline { get; set; }
        public String Source { get; set; }
        public String ShortLink { get; set; }
        public DateTime DateOfRegisration { get; set; }
        public Guid NewsCategoryId { get; set; }

        public NewsCategory? NewsCategory { get; set; }
    }
}
