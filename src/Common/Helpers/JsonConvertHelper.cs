using Common.Extensions;
using System.Diagnostics;
using System.Text.Json;

namespace Common.Helpers;

public static class JsonConvertHelper
{
    public static T? Deserialize<T>(this string? json)
    {
        try
        {
            if(json.IsNullOrWhiteSpace())
            {
                return default(T?);
            }

            return JsonSerializer.Deserialize<T>(json ?? string.Empty);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return default;
        }
    }

    public static string? Serialize<T>(this T? data)
    {
        try
        {
            if (data == null)
            {
                return string.Empty;
            }

            return JsonSerializer.Serialize(data);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return string.Empty;
        }
    }
}