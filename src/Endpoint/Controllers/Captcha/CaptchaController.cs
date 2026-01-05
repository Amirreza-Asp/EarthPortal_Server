using Application.CQRS.Captcha;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Captcha
{
    [Route("api/Captcha")]
    [ApiController]
    public class CaptchaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CaptchaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetCaptcha")]
        public async Task<IActionResult> GetCaptcha(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GenerateCaptchaCommandRequest(),
                cancellationToken
            );
            return File(result, "Image/Png", true);
        }
    }
}
