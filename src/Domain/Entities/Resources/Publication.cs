using Domain.Shared;

namespace Domain.Entities.Resources
{
    public class Publication : BaseEntity
    {
        public Publication(string title, string? description)
        {
            Title = title;
            Description = description;
        }

        public String Title { get; set; }
        public String? Description { get; set; }
    }
}
