using Domain.Shared;

namespace Domain.Entities.Regulation.ValueObjects
{
    //ابلاغیه
    public class Announcement : BaseValueObject<Announcement>
    {
        public Announcement(string number, DateTime date)
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

        public override bool ObjectIsEqual(Announcement other)
        {
            return Number == other.Number && Date == other.Date;
        }
    }
}
