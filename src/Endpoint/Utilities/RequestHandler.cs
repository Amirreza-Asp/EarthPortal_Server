﻿using Application.Models;
using MediatR;

namespace Endpoint.Utilities
{
    public static class RequestHandler
    {

        public static async Task<CommandResponse> HandleRequestAsync<TCommand>(this IMediator mediator, TCommand command, CancellationToken cancellationToken = default) where TCommand : IRequest<CommandResponse>
        {
            try
            {
                return await mediator.Send(command, cancellationToken);
            }
            catch (Exception ex)
            {
                return CommandResponse.Failure(500, ex);
            }
        }


    }
}
