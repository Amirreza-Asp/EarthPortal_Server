using Domain.Shared;

namespace Domain.Entities.Resources
{
    public class Publication : BaseEntity
    {
        public Publication(string title)
        {
            Title = title;
        }

        public String Title { get; set; }
    }
}
