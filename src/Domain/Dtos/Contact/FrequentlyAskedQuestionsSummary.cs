namespace Domain.Dtos.Content
{
    public class FrequentlyAskedQuestionsSummary
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Content { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
