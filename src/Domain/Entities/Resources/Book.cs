using Domain.Shared;

namespace Domain.Entities.Resources
{
    public class Book : BaseEntity
    {
        public Book(string title, string? description, string path, DateTime publishDate, Guid authorId)
        {
            Title = title;
            Description = description;
            Path = path;
            PublishDate = publishDate;
            AuthorId = authorId;
        }

        private Book() { }

        public String Title { get; set; }
        public String? Description { get; set; }
        public String Path { get; set; }
        public DateTime PublishDate { get; set; }
        public Guid AuthorId { get; set; }


        public Author? Author { get; set; }
    }
}
