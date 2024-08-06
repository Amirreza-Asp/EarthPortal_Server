using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class Statistics : BaseEntity
    {
        public Statistics(Guid id, int seen, DateTime date) : base(id)
        {
            Seen = seen;
            Date = date;
        }

        public int Seen { get; set; }
        public DateTime Date { get; set; }


    }
}
