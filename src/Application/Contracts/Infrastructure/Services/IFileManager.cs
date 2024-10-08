﻿using Microsoft.AspNetCore.Http;

namespace Application.Contracts.Infrastructure.Services
{
    public interface IFileManager
    {
        double GetSize(String path, FileSize fileSize);

        Task SaveFileAsync(IFormFile file, string path);

        void ConvertHtmlToPdf(string htmlContent, string outputPath);
    }



    public enum FileSize
    {
        KB = 1,
        MB = 2,
        GB = 3,
    }
}
