using Application.Contracts.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Services
{
    public class PhotoManager : IPhotoManager
    {
        public void Delete(string path, CancellationToken cancellationToken = default)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public async Task<Byte[]> ResizeAsync(string path, int? width, int? height, CancellationToken cancellationToken = default)
        {
            using var image = await Image.LoadAsync(path, cancellationToken);

            if (width.HasValue && height.HasValue)
                image.Mutate(b => b.Resize(width.Value, height.Value));

            using (var ms = new MemoryStream())
            {
                var encoder = image.DetectEncoder(path);
                image.Save(ms, encoder);
                return ms.ToArray();
            }
        }

        public async Task SaveAsync(IFormFile file, String path, CancellationToken cancellationToken = default)
        {
            var image = await Image.LoadAsync(file.OpenReadStream(), cancellationToken);
            await image.SaveAsync(path, cancellationToken);
        }

        public async Task SaveFromBase64Async(string base64File, string path, CancellationToken cancellationToken = default)
        {
            var bytes = Convert.FromBase64String(base64File);
            await File.WriteAllBytesAsync(path, bytes, cancellationToken);
        }
    }
}
