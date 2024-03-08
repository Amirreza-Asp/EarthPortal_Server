using Microsoft.AspNetCore.Http;

namespace Application.Contracts.Infrastructure.Services
{
    public interface IPhotoManager
    {
        Task<Byte[]> ResizeAsync(string path, int? width, int? height, CancellationToken cancellationToken = default);

        Task SaveAsync(IFormFile file, String path, CancellationToken cancellationToken = default);
        Task SaveFromBase64Async(String base64File, String path, CancellationToken cancellationToken = default);

        void Delete(String path, CancellationToken cancellationToken = default);
    }
}
