using Domain.Shared;

namespace Domain.Entities.Regulation
{
    public class LawCategory : BaseEntity
    {
        public LawCategory(string title)
        {
            Title = title;
        }

        public String Title { get; set; }
    }
}
