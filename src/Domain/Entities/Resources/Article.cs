using Domain.Shared;

namespace Domain.Entities.Resources
{
    public class Article : BaseEntity
    {
        public Article(string title, string? description, string path, Guid authorId, DateTime publishDate, string source)
        {
            Title = title;
            Description = description;
            Path = path;
            AuthorId = authorId;
            PublishDate = publishDate;
            Source = source;
        }

        public String Title { get; set; }
        public String? Description { get; set; }
        public String Path { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime PublishDate { get; set; }
        public String Source { get; set; }


        public Author? Author { get; set; }

    }
}
