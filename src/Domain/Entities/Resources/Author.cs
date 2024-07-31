using Domain.Shared;

namespace Domain.Entities.Resources
{
    public class Author : BaseEntity
    {
        public Author(string fullName) : base(Guid.NewGuid())
        {
            FullName = fullName;
        }

        private Author() { }

        public String FullName { get; set; }
    }
}
