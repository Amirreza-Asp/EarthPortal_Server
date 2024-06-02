using Domain.Shared;

namespace Domain.Entities.Regulation.ValueObjects
{
    public class Newspaper : BaseValueObject<Newspaper>
    {
        private Newspaper()
        {
        }

        public static Newspaper Create(string number, DateTime date)
        {
            return new Newspaper() { Number = number, Date = date };
        }

        public string Number { get; set; }
        public DateTime Date { get; set; }

        public override int ObjectGetHashCode()
        {
            return Number.GetHashCode() + Date.GetHashCode();
        }

        public override bool ObjectIsEqual(Newspaper other)
        {
            return Number == other.Number && Date == other.Date;
        }
    }
}
