using Domain.Shared;

namespace Domain.Entities.Regulation.ValueObjects
{
    public class Newspaper : BaseValueObject<Newspaper>
    {
        public Newspaper(string number, DateTime date)
        {
            Number = number;
            Date = date;
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
