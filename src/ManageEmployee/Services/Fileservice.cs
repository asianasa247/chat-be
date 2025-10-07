using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.Services.Interfaces.Assets;

namespace ManageEmployee.Services;

public class FileService : IFileService
{
    public string Upload(IFormFile file, string folder = "Images", string fileNameUpload = null)
    {
        var uploadDirecotroy = "Uploads\\";
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), uploadDirecotroy);

        uploadDirecotroy += folder;
        uploadPath = Path.Combine(uploadPath, folder);

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);
        var fileName = (!string.IsNullOrEmpty(fileNameUpload) ? fileNameUpload : (Guid.NewGuid().ToString() + Path.GetExtension(file.FileName)));
        var filePath = Path.Combine(uploadPath, fileName);

        using (var stream = File.Create(filePath))
        {
            file.CopyTo(stream);
        }

        return Path.Combine(uploadDirecotroy, fileName);
    }

    public async Task<string> UploadBase64(string base64String, string folder = "Images", string fileNameUpload = null)
    {
        try
        {
            // Lo?i b? ti?n t? "data:image/png;base64,"
            string base64Data = base64String.Split(',')[1];

            byte[] imageBytes = Convert.FromBase64String(base64Data);
            using (var stream = new MemoryStream(imageBytes))
            {
                return await UploadStream(stream, folder, fileNameUpload);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"L?i khi upload base64: {ex.Message}");
            return null;
        }
    }

    private async Task<string> UploadStream(Stream fileStream, string folder = "Images", string fileNameUpload = null)
    {
        var uploadDirectory = "Uploads";
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), uploadDirectory, folder);

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        var fileName = !string.IsNullOrEmpty(fileNameUpload) ? fileNameUpload : $"{Guid.NewGuid()}.jpg";
        var filePath = Path.Combine(uploadPath, fileName);

        using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await fileStream.CopyToAsync(file);
        }

        return Path.Combine(uploadDirectory, folder, fileName).Replace("\\", "/");
    }

    public FileDetailModel UploadFile(IFormFile file, string folder, string fileNameUpload)
    {
        var uploadDirecotroy = "Uploads\\";
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), uploadDirecotroy);
        uploadDirecotroy += folder;

        uploadPath = Path.Combine(uploadPath, folder);

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadPath, fileName);

        using (var stream = File.Create(filePath))
        {
            file.CopyTo(stream);
        }

        return new FileDetailModel
        {
            FileUrl = Path.Combine(uploadDirecotroy, fileName),
            FileName = fileNameUpload
        };
            
    }

    public bool DeleteFileUpload(string filePath)
    {
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
        if (File.Exists(uploadPath))
        {
            File.Delete(uploadPath);
        }
        return true;
    }
}