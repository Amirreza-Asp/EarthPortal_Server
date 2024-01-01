using Domain.Shared;

namespace Domain.Entities.Notices
{
    public class NewsAttachedFile : BaseEntity
    {
        public NewsAttachedFile(string path, Guid newsId)
        {
            Id = Guid.NewGuid();
            Path = path;
            NewsId = newsId;
        }

        private NewsAttachedFile() { }

        public String Path { get; set; }
        public Guid NewsId { get; set; }

        public News? News { get; set; }
    }
}
