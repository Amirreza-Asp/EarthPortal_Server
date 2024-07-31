using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class RelatedLink : BaseEntity
    {
        public RelatedLink(string title, string link, int order) : base(Guid.NewGuid())
        {
            Title = title;
            Link = link;
            Order = order;
        }

        private RelatedLink() { }

        public String Title { get; set; }
        public String Link { get; set; }
    }
}
