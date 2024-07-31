using Domain.Shared;

namespace Domain.Dtos.Resources
{
    public class Translator : BaseEntity
    {
        public Translator(string fullName) : base(Guid.NewGuid())
        {
            FullName = fullName;
        }

        private Translator()
        {

        }

        public String FullName { get; set; }
    }
}
