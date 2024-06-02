namespace Domain.Dtos.Notices
{
    public class NewsCategorySummary
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
