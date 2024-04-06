using Domain.Dtos.Notices;
using Domain.Entities.Contact;

namespace Domain.Dtos.Shared
{
    public class HomeSummary
    {
        public List<NewsSummary> News { get; set; }
        public List<RelatedCompany> Companies { get; set; }
        public List<Goal> Goals { get; set; }
    }
}
