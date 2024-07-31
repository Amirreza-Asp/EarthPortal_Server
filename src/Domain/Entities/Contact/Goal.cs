using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class Goal : BaseEntity
    {
        public Goal(string title) : base(Guid.NewGuid())
        {
            Title = title;
        }

        private Goal() { }

        public String Title { get; set; }
    }
}
