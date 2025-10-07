namespace ManageEmployee.Extends;

public static class IFormFileExtension
{
    public static async Task<(byte[] FileBytes, string FileName, string FileExtension)> ParseToBytes(this IFormFile formFile)
    {
        await using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);
        return (memoryStream.ToArray(), formFile.FileName, Path.GetExtension(formFile.FileName));
    }
}
