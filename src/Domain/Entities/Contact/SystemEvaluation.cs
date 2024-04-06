using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class SystemEvaluation : BaseEntity
    {
        public SystemEvaluation(int score, string ip)
        {
            Score = score;
            Ip = ip;
        }

        public int Score { get; set; }
        public String Ip { get; set; }
    }
}
