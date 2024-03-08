using Domain.Shared;

namespace Domain.Entities.Notices
{
    public class Link : BaseEntity
    {
        public Link(string value)
        {
            Value = value;
        }

        private Link() { }

        public String Value { get; set; }

        public ICollection<NewsLink> NewsLinks { get; set; }
    }
}
