using Domain.Shared;

namespace Domain.Entities.Notices
{
    public class Link : BaseEntity
    {
        public Link(string value, Guid newsId)
        {
            Id = Guid.NewGuid();
            Value = value;
            NewsId = newsId;
        }

        private Link() { }

        public String Value { get; set; }
        public Guid NewsId { get; set; }

        public News? News { get; set; }
    }
}
