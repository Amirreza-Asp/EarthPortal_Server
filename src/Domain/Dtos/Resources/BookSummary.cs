namespace Domain.Dtos.Resources
{
    public class BookSummary
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Image { get; set; }
        public String ShortDescription { get; set; }
        public String Author { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
