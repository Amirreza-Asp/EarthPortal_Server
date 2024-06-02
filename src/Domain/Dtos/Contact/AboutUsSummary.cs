namespace Domain.Dtos.Contact
{
    public class AboutUsSummary
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Content { get; set; }
        public String? Image { get; set; }
        public String? Video { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool HaveVideo { get; set; }
        public int Order { get; set; }
    }
}
