﻿using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.CQRS.Notices
{
    public class CreateNewsCommand : IRequest<CommandResponse>
    {
        public String Title { get; set; }

        public String Description { get; set; }

        public String Headline { get; set; }

        public String Source { get; set; }

        public DateTime DateOfRegisration { get; set; }


        public List<String>? Links { get; set; }

        [Required(ErrorMessage = "تصویر خبر را وارد کنید")]
        public IFormFile Image { get; set; }
        public int Order { get; set; }
    }
}
