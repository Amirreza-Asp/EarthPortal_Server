using Application.Contracts.Infrastructure.Services;

namespace Infrastructure.Services
{
    public class FileManager : IFileManager
    {
        public double GetSize(string path, FileSize fileSize)
        {
            var fileInfo = new FileInfo(path);
            var size = fileInfo.Length;

            switch (fileSize)
            {
                case FileSize.KB: return size / 1024;
                case FileSize.MB: return size / Math.Pow(1024, 2);
                case FileSize.GB: return size / Math.Pow(1024, 3);
                default: return size;
            }
        }
    }
}
