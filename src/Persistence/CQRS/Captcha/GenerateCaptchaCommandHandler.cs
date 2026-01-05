using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Captcha;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using SkiaSharp;

namespace Persistence.CQRS.Captcha;

public class GenerateCaptchaCommandHandler : IRequestHandler<GenerateCaptchaCommandRequest, byte[]>
{
    private readonly IWebHostEnvironment _environment;
    private readonly ISessionService _sessionService;

    public GenerateCaptchaCommandHandler(
        IWebHostEnvironment environment,
        ISessionService sessionService
    )
    {
        _environment = environment;
        _sessionService = sessionService;
    }

    public async Task<byte[]> Handle(
        GenerateCaptchaCommandRequest request,
        CancellationToken cancellationToken
    )
    {
        _sessionService.RemoveSession("captcha");
        Random random = new Random();
        string captchaCode = GenerateRandomCode(6);
        _sessionService.SetSession("captcha", captchaCode);

        byte[] captchaBytes = await GenerateCaptchaImage(captchaCode);
        return captchaBytes;
    }

    private string GenerateRandomCode(int length)
    {
        // ایجاد یک کد تصادفی با حروف و اعداد
        Random random = new Random();
        const string characters = "0123456789";
        return new string(
            Enumerable.Repeat(characters, length).Select(s => s[random.Next(s.Length)]).ToArray()
        );
    }

    private async Task<byte[]> GenerateCaptchaImage(string code)
    {
        // تعریف متغیر برای فونت
        SKTypeface customTypeface;

        // مسیر فایل فونت را تعیین کنید
        string fontPath = Path.Combine(_environment.WebRootPath, "fonts", "B-NAZANIN.TTF");
        ; // مسیر فایل فونت در پروژه

        // بارگذاری فونت از فایل
        using (var fontStream = System.IO.File.OpenRead(fontPath))
        {
            customTypeface = SKTypeface.FromStream(fontStream);
        }

        using (var surface = SKSurface.Create(new SKImageInfo(110, 40)))
        using (var canvas = surface.Canvas)
        {
            canvas.Clear(SKColors.WhiteSmoke);

            using (var paint = new SKPaint())
            {
                // استفاده از فونت سفارشی
                paint.Typeface = customTypeface;
                paint.TextSize = 32;
                paint.Color = SKColors.Black;
                canvas.DrawText(code, 10, 30, paint);
            }

            var random = new Random();

            int numberOfRandomLines = random.Next(5, 10); // تعداد خطوط تصادفی
            SKColor[] possibleColors = new SKColor[]
            {
                SKColors.Red,
                SKColors.Blue,
                SKColors.Green,
                SKColors.Orange,
                SKColors.Purple
            };

            for (int i = 0; i < numberOfRandomLines; i++)
            {
                SKPoint startPoint = new SKPoint(random.Next(100), random.Next(50));
                SKPoint endPoint = new SKPoint(random.Next(100), random.Next(50));
                SKColor lineColor = possibleColors[random.Next(possibleColors.Length)];

                using (var linePaint = new SKPaint())
                {
                    linePaint.Color = lineColor;
                    linePaint.StrokeWidth = 1;

                    canvas.DrawLine(startPoint, endPoint, linePaint);
                }
            }

            int numberOfRandomPoints = 100; // تعداد نقاط تصادفی

            for (int i = 0; i < numberOfRandomPoints; i++)
            {
                SKPoint point = new SKPoint(random.Next(100), random.Next(50));
                SKColor pointColor = possibleColors[random.Next(possibleColors.Length)];

                using (var pointPaint = new SKPaint())
                {
                    pointPaint.Color = pointColor;

                    canvas.DrawPoint(point, pointPaint);
                }
            }

            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = new MemoryStream(data.ToArray()))
            {
                return stream.ToArray();
            }
        }
    }
}
