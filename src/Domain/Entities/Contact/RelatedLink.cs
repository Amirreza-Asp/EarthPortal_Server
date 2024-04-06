using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class RelatedLink : BaseEntity
    {
        public RelatedLink(string title, string link)
        {
            Title = title;
            Link = link;
        }

        public String Title { get; set; }
        public String Link { get; set; }
    }
}
