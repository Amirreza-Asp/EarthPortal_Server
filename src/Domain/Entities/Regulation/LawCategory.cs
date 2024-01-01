using Domain.Shared;

namespace Domain.Entities.Regulation
{
    public class LawCategory : BaseEntity
    {
        public LawCategory(string title, string? description)
        {
            Title = title;
            Description = description;
        }

        public String Title { get; set; }
        public String? Description { get; set; }
    }
}
