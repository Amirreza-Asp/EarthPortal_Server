using Domain.Shared;

namespace Domain.Entities.Notices
{
    public class NewsImage : BaseEntity
    {
        public NewsImage(string path, Guid newsId, int order)
        {
            Path = path;
            NewsId = newsId;
            Order = order;
        }

        private NewsImage() { }

        public String Path { get; set; }
        public Guid NewsId { get; set; }
        public int Order { get; set; }

        public News? News { get; set; }
    }
}
