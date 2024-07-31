using Domain.Shared;

namespace Domain.Entities.Notices
{
    public class NewsImage : BaseEntity
    {
        public NewsImage(string name, Guid newsId, int order) : base(Guid.NewGuid())
        {
            Name = name;
            NewsId = newsId;
            Order = order;
        }

        private NewsImage() { }

        public String Name { get; set; }
        public Guid NewsId { get; set; }
        public int Order { get; set; }

        public News? News { get; set; }
    }
}
