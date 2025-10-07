namespace Common.Extensions;

public static class FileExtension
{
    public static Task<string> ReadContent(params string[] paths)
    {
        var path = Path.Combine(paths);
        return File.ReadAllTextAsync(path);
    }
}
