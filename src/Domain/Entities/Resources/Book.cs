using Domain.Dtos.Resources;
using Domain.Shared;

namespace Domain.Entities.Resources
{
    public class Book : BaseEntity
    {
        public Book(string title, string? description, string file, DateTime publishDate, Guid authorId, string image, string shortDescription, int pages, Guid translatorId, Guid publicationId)
        {
            Title = title;
            Description = description;
            File = file;
            PublishDate = publishDate;
            AuthorId = authorId;
            Image = image;
            ShortDescription = shortDescription;
            Pages = pages;
            TranslatorId = translatorId;
            PublicationId = publicationId;
        }

        private Book() { }

        public String Title { get; set; }
        public String ShortDescription { get; set; }
        public String? Description { get; set; }
        public String Image { get; set; }
        public String File { get; set; }
        public DateTime PublishDate { get; set; }
        public int Pages { get; set; }

        public Guid AuthorId { get; set; }
        public Guid TranslatorId { get; set; }
        public Guid PublicationId { get; set; }

        public Publication? Publication { get; set; }
        public Translator? Translator { get; set; }
        public Author? Author { get; set; }
    }
}
