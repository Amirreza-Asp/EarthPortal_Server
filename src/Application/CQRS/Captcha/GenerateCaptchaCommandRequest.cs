using MediatR;

namespace Application.CQRS.Captcha;

public class GenerateCaptchaCommandRequest : IRequest<byte[]> { }
