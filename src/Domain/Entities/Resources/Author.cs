using Domain.Shared;

namespace Domain.Entities.Resources
{
    public class Author : BaseEntity
    {
        public Author(string fullName)
        {
            FullName = fullName;
        }

        private Author() { }

        public String FullName { get; set; }
    }
}
