using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class Goal : BaseEntity
    {
        public Goal(string title)
        {
            Title = title;
        }

        public String Title { get; set; }
    }
}
