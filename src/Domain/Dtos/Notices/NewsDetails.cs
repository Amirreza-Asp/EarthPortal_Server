namespace Domain.Dtos.Notices
{
    public class NewsDetails
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Image { get; set; }
        public String Headline { get; set; }
        public String Description { get; set; }
        public long Seen { get; set; }
        public String Source { get; set; }
        public DateTime DateOfRegisration { get; set; }
        public Guid NewsCategoryId { get; set; }
        public List<Keyword> Keywords { get; set; }
        public List<NewsSummary> RelatedNews { get; set; }
    }
}
