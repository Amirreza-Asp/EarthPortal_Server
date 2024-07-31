using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class SystemEvaluation : BaseEntity
    {
        public SystemEvaluation(int score, string ip) : base(Guid.NewGuid())
        {
            Score = score;
            Ip = ip;
        }

        private SystemEvaluation() { }

        public int Score { get; set; }
        public String Ip { get; set; }
    }
}
