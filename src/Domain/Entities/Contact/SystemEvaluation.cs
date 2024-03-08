using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class SystemEvaluation : BaseEntity
    {
        public SystemEvaluation(int score)
        {
            Score = score;
        }

        public int Score { get; set; }
    }
}
