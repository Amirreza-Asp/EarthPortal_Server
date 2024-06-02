namespace Domain.Dtos.Resources
{
    public class ArticleDetails
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String ShortDescription { get; set; }
        public String? Description { get; set; }
        public String Image { get; set; }
        public String File { get; set; }
        public DateTime PublishDate { get; set; }
        public String Author { get; set; }
        public String Translator { get; set; }
        public String Publication { get; set; }
        public Guid PublicationId { get; set; }
        public Guid TranslatorId { get; set; }
        public Guid AuthorId { get; set; }
        public int Pages { get; set; }
        public double Size { get; set; }
        public int Order { get; set; }
    }
}
