using Domain.Shared;

namespace Domain.Entities.Regulation
{
    public class LawCategory : BaseEntity
    {
        public LawCategory(string title) : base(Guid.NewGuid())
        {
            Title = title;
        }

        private LawCategory()
        {

        }

        public String Title { get; set; }
    }
}
