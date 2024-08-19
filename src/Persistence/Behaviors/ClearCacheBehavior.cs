using Application.CQRS.Regulation;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Persistence.Behavoirs
{
    public class ClearCacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IMemoryCache _cacheService;

        public ClearCacheBehavior(IMemoryCache cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            // Clear cache if the request is related to Regulation
            if (request is IRegulationCommand)
            {
                _cacheService.Remove("laws");
            }

            return response;
        }
    }
}
