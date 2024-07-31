using Domain.Shared;

namespace Domain.Entities.Resources
{
    public class Publication : BaseEntity
    {
        public Publication(string title) : base(Guid.NewGuid())
        {
            Title = title;
        }

        private Publication()
        {

        }

        public String Title { get; set; }
    }
}
