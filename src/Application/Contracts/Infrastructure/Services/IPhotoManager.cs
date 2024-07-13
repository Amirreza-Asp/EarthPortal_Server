using Microsoft.AspNetCore.Http;

namespace Application.Contracts.Infrastructure.Services
{
    public interface IPhotoManager
    {
        Task<Byte[]> ResizeAsync(string path, int? width, int? height, CancellationToken cancellationToken = default);

        void Save(IFormFile file, String path);
        Task SaveFromBase64Async(String base64File, String path, CancellationToken cancellationToken = default);

        void Delete(String path, CancellationToken cancellationToken = default);
    }
}
