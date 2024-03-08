using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.SystemEvaluations
{
    public class CreateSystemEvaluationCommand : IRequest<CommandResponse>
    {
        public List<int> IntroductionMethods { get; set; }
        public List<int> Pages { get; set; }
        public int Score { get; set; }
    }
}
