using Domain.Entities.Contact.Enums;
using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class SystemEvaluationIntroductionMethod : BaseEntity
    {
        public SystemEvaluationIntroductionMethod(IntroductionMethod introductionMethod, SystemEvaluation systemEvaluation)
        {
            IntroductionMethod = introductionMethod;
            SystemEvaluation = systemEvaluation;
        }

        private SystemEvaluationIntroductionMethod()
        {
        }

        public IntroductionMethod IntroductionMethod { get; set; }
        public Guid SystemEvaluationId { get; set; }

        public SystemEvaluation SystemEvaluation { get; set; }
    }
}
