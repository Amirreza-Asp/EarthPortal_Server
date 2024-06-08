using Application.Contracts.Infrastructure.Services;
using Aspose.Pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class FileManager : IFileManager
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _hostEnv;

        public FileManager(IHttpContextAccessor contextAccessor, IWebHostEnvironment hostEnv)
        {
            _contextAccessor = contextAccessor;
            _hostEnv = hostEnv;
        }

        public void ConvertHtmlToPdf(string htmlContent, string outputPath)
        {
            try
            {
                // Initialize document object
                Document document = new Document();
                document.Direction = Direction.R2L;
                // Add page
                Page page = document.Pages.Add();

                var text = new Aspose.Pdf.Text.TextFragment(htmlContent);
                text.HorizontalAlignment = HorizontalAlignment.Right;
                text.Margin = new MarginInfo() { Right = 10, Left = 10 };

                page.Paragraphs.Add(text);
                // Save updated PDF
                document.Save(outputPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

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

        public async Task SaveFileAsync(IFormFile file, string path)
        {
            using (Stream fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }
    }
}
