using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Captcha
{
    public class GenerateCaptchaCommandRequest : IRequest<byte[]>
    {
    }
}
