using Microsoft.AspNetCore.Http;

namespace ViNgocHiep_2123110365.Helpers
{
    public static class FileHelper
    {
        public static async Task<string?> UploadFileAsync(IFormFile? file, string folderName)
        {
            if (file == null)
                return null;

            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                folderName
            );
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var randomString = Guid.NewGuid().ToString("N").Substring(0, 8);
            var fileExtension = Path.GetExtension(file.FileName);

            var fileName = $"{timestamp}_{randomString}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{folderName}/{fileName}";
        }
    }
}
