namespace Application.Contracts.Infrastructure.Services
{
    public interface IFileManager
    {
        double GetSize(String path, FileSize fileSize);
    }

    public enum FileSize
    {
        KB = 1,
        MB = 2,
        GB = 3,
    }
}
