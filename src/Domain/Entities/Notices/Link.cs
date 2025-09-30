using Domain.Shared;

namespace Domain.Entities.Notices
{
    public class Link : BaseEntity
    {
        public Link(Guid id, string value) : base(id)
        {
            Value = value;
        }

        private Link() { }

        public String Value { get; set; }

        public ICollection<NewsLink> NewsLinks { get; set; }
        public ICollection<NoticeLink> NoticeLinks { get; set; }
    }
}
