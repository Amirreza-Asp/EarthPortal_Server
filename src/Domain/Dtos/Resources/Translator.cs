using Domain.Shared;

namespace Domain.Dtos.Resources
{
    public class Translator : BaseEntity
    {
        public Translator(string fullName)
        {
            FullName = fullName;
        }

        public String FullName { get; set; }
    }
}
