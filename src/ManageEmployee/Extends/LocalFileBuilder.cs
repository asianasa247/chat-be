using Common.Extensions;

namespace ManageEmployee.Extends;

public record UploadFileResult(
    string FileName,
    string FileExtension,
    string FilePath,
    int FileSize
);

public class LocalFileBuilder
{
    private string? _rootFolder;
    private string? _folderUpload;

    public LocalFileBuilder SetRootFolder(string rootFolder)
    {
        _rootFolder = rootFolder;
        return this;
    }

    public LocalFileBuilder SetFolderUpload(string folderUpload)
    {
        _folderUpload = folderUpload;
        return this;
    }

    public Task<(byte[] FileBytes, string FileName, string FileExtension)> ParseToBytes(IFormFile formFile)
    {
        if (formFile == null) 
        {
            throw new ArgumentNullException(nameof(formFile));
        }
        return formFile.ParseToBytes();
    }

    public async Task<UploadFileResult> UploadFile(IFormFile formFile, string? fileUpload = default)
    {
        var (FileBytes, FileName, FileExtension) = await ParseToBytes(formFile);

        var rootFolder = _rootFolder.DefaultIfNullOrEmpty(Constants.Constants.RootFolderUpload);

        var folderUpload = !string.IsNullOrWhiteSpace(_folderUpload) 
            ? Path.Combine(rootFolder, _folderUpload)
            : rootFolder;

        if (!Directory.Exists(folderUpload))
            Directory.CreateDirectory(folderUpload);

        var fileName = GetFileUpload(formFile, fileUpload);

        var filePath = Path.Combine(folderUpload, fileName);

        await WriteFile(filePath);

        return new UploadFileResult(fileName, FileExtension, filePath, FileBytes.Length);
    }

    private static string GetFileUpload(IFormFile formFile, string? fileUpload)
    {
        if(string.IsNullOrWhiteSpace(fileUpload))
        {
            return $"{DateTime.Now.Ticks}_{formFile.FileName}";
        }

        var fileUploadExtension = Path.GetExtension(fileUpload);
        return string.IsNullOrWhiteSpace(fileUploadExtension)
            ? $"{fileUpload}{Path.GetExtension(formFile.FileName)}"
            : fileUpload;
    }

    private static async Task WriteFile(string filePath)
    {
        await using var stream = File.OpenWrite(filePath);
        stream.Position = 0;
        await stream.CopyToAsync(stream);
    }
}
