using Domain.Shared;

namespace Domain.Entities.Accounting
{
    public class Role : BaseEntity
    {
        public Role(string title, string? description)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
        }


        public String Title { get; set; }
        public String? Description { get; set; }
    }
}
