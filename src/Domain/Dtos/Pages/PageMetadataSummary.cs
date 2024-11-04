using Domain.Entities.Pages;

namespace Domain.Dtos.Pages
{
    public class PageMetadataSummary
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public Page Page { get; set; }
        public IList<String> Keywords { get; set; }
    }
}
