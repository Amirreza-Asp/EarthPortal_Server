﻿using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.Categories
{
    public class CreateCategoryCommand : IRequest<CommandResponse>, IRegulationCommand
    {
        public String Title { get; set; }
        public int Order { get; set; }
    }
}
