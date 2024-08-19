using Domain.Shared;

namespace Domain.Entities.Regulation.ValueObjects
{
    public class Newspaper : BaseValueObject<Newspaper>
    {
        private Newspaper()
        {
        }

        public static Newspaper Create(string number, DateTime date, String File)
        {
            return new Newspaper() { Number = number, Date = date, File = File };
        }

        public string Number { get; private set; }
        public DateTime Date { get; private set; }
        public String File { get; private set; }

        public override int ObjectGetHashCode()
        {
            return Number.GetHashCode() & Date.GetHashCode();
        }

        public override bool ObjectIsEqual(Newspaper other)
        {
            return Number == other.Number && Date == other.Date && File == other.File;
        }
    }
}
