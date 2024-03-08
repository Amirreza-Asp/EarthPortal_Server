namespace Application.Queries
{
    public class NewsFilteringPagenationQuery
    {
        public List<Guid> LinksId { get; set; } = new List<Guid>();
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public String Title { get; set; }
    }
}
