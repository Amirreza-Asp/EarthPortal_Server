using Domain.Shared;

namespace Domain.Entities.Resources
{
    public class Article : BaseEntity
    {
        public Article(string title, string? description, Guid authorId, DateTime publishDate, string file, string shortDescription, string image)
        {
            Title = title;
            Description = description;
            AuthorId = authorId;
            PublishDate = publishDate;
            File = file;
            ShortDescription = shortDescription;
            Image = image;
        }

        private Article() { }

        public String Title { get; set; }
        public String? Description { get; set; }
        public DateTime PublishDate { get; set; }
        public String ShortDescription { get; set; }
        public String File { get; set; }
        public String Image { get; set; }
        public Guid AuthorId { get; set; }


        public Author? Author { get; set; }

    }
}
