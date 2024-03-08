using Domain.Shared;

namespace Domain.Entities.Notices
{
    public class NewsCategory : BaseEntity
    {
        public NewsCategory(string title, string? description)
        {
            Title = title;
            Description = description;
        }

        private NewsCategory() { }

        public String Title { get; set; }
        public String? Description { get; set; }
    }
}
