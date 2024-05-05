using Application.Contracts.Infrastructure.Services;
using Aspose.Pdf;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class FileManager : IFileManager
    {
        public void ConvertHtmlToPdf(string htmlContent, string outputPath)
        {
            try
            {
                //var renderer = new ChromePdfRenderer();
                //PdfDocument pdf = renderer.RenderHtmlAsPdf($"<p>{htmlContent}</p>");
                //pdf.PrintToFile(outputPath);

                // Initialize document object
                Document document = new Document();
                // Add page
                Page page = document.Pages.Add();
                // Add text to new page
                page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment(htmlContent));
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
