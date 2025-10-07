namespace ManageEmployee.Helpers;

public static class FileHelper
{
    public static string GetAbsolutePath(this string relativePath)
    {
        return $@"{Directory.GetCurrentDirectory()}\{relativePath}";
    }
    
    public static string CreateDirectoryIfNotExists(this string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        return folderPath;
    }
}