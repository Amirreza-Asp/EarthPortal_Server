﻿using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.CQRS.Notices
{
    public class UpdateNewsCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }

        public String Title { get; set; }

        public String Description { get; set; }

        public String Headline { get; set; }

        public String Source { get; set; }

        public DateTime DateOfRegisration { get; set; }

        public Guid NewsCategoryId { get; set; }

        public List<String> Links { get; set; }

        public IFormFile? Image { get; set; }
    }

}
