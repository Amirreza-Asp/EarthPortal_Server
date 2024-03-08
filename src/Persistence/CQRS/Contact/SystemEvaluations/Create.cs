using Application.CQRS.Contact.SystemEvaluations;
using Application.Models;
using Domain.Entities.Contact;
using Domain.Entities.Contact.Enums;
using MediatR;

namespace Persistence.CQRS.Contact.SystemEvaluations
{
    public class CreateSystemEvaluationCommandHandler : IRequestHandler<CreateSystemEvaluationCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateSystemEvaluationCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateSystemEvaluationCommand request, CancellationToken cancellationToken)
        {
            var systemEvaluation = new SystemEvaluation(request.Score);

            var systemEvaluationPages = new List<SystemEvaluationPage>();
            request.Pages.ForEach(page =>
                 {
                     systemEvaluationPages.Add(new SystemEvaluationPage((Pages)page, systemEvaluation.Id));
                 });

            _context.SystemEvaluationPage.AddRange(systemEvaluationPages);


            var introductionMethods = new List<SystemEvaluationIntroductionMethod>();
            request.IntroductionMethods.ForEach(method =>
            {
                introductionMethods.Add(new SystemEvaluationIntroductionMethod((IntroductionMethod)method, systemEvaluation));
            });

            _context.IntroductionMethod.AddRange(introductionMethods);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
