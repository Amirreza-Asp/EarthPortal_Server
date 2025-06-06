﻿using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.Laws
{
    public class RemoveLawCommand : IRequest<CommandResponse>, IRegulationCommand
    {
        public Guid Id { get; set; }
    }
}
