namespace Domain.Entities.Notices
{
    public class NewsLink
    {
        public NewsLink(Guid newsId, Guid linkId)
        {
            NewsId = newsId;
            LinkId = linkId;
        }

        private NewsLink() { }

        public Guid NewsId { get; set; }
        public Guid LinkId { get; set; }

        public News? News { get; set; }
        public Link? Link { get; set; }
    }
}
