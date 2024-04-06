using Application.CQRS.Contact.SystemEvaluations;
using Application.Models;
using Domain.Entities.Contact;
using Domain.Entities.Contact.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.SystemEvaluations
{
    public class CreateSystemEvaluationCommandHandler : IRequestHandler<CreateSystemEvaluationCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public CreateSystemEvaluationCommandHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<CommandResponse> Handle(CreateSystemEvaluationCommand request, CancellationToken cancellationToken)
        {
            var ip = _contextAccessor.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();

            if (await _context.SystemEvaluation.AnyAsync(b => b.Ip == ip, cancellationToken))
                return CommandResponse.Failure(400, "شما قبلا در ارزیابی شرکت کرده اید");


            var systemEvaluation = new SystemEvaluation(request.Score, ip);

            _context.SystemEvaluation.Add(systemEvaluation);

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
