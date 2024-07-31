using Domain.Shared;

namespace Domain.Entities.Account
{
    public class Role : BaseEntity
    {
        public Role(string title, string display, string? description) : base(Guid.NewGuid())
        {
            Title = title;
            Description = description;
            Display = display;
        }

        private Role() { }


        public string Title { get; set; }
        public String Display { get; set; }
        public string? Description { get; set; }
    }
}
