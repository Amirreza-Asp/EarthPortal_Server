namespace Domain.Dtos.Contact
{
    public class GuideSummary
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public bool IsPort { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
