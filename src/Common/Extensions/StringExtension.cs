using System.Text.RegularExpressions;
using System.Text;

namespace Common.Extensions;
public static class StringExtension
{
    public static bool IsNullOrEmpty(this string? input) => string.IsNullOrEmpty(input);
    public static bool IsNullOrWhiteSpace(this string? input) => string.IsNullOrWhiteSpace(input);
    public static string ConvertToUnSign(this string input)
    {
        if (input.IsNullOrEmpty())
            return null;

        input = input.Trim();
        for (int i = 0x20; i < 0x30; i++)
        {
            input = input.Replace(((char)i).ToString(), " ");
        }
        Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
        string str = input.Normalize(NormalizationForm.FormD);
        string str2 = regex.Replace(str, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');
        while (str2.IndexOf("?") >= 0)
        {
            str2 = str2.Remove(str2.IndexOf("?"), 1);
        }
        return str2.ToLower();
    }
    public static string RemoveAccents(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return "";
        }

        string[] _vietNamChar = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };
        var result = input.ToLower();
        for (int i = 1; i < _vietNamChar.Length; i++)
        {
            for (int j = 0; j < _vietNamChar[i].Length; j++)
                result = result.Replace(_vietNamChar[i][j], _vietNamChar[0][i - 1]);
        }
        return result;
    }

    public static string DefaultIfNullOrEmpty(this string? source, string? defaultVal)
    {
        var result = source == null || source.IsNullOrEmpty() || source.IsNullOrWhiteSpace() ? defaultVal : source;
        return result ?? string.Empty;
    }

    public static string FilePathAsUrl(this string? path)
    {
        path = path.DefaultIfNullOrEmpty("");
        return path.Replace('\\', '/');
    }

    public static string JoinString(this List<string> list, string separator)
    {
        return string.Join(separator, list.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    public static string DefaultIfEmpty(this string val, string defaultValue) 
    {
        return  string.IsNullOrWhiteSpace(val) ? defaultValue : val;
    }
}
