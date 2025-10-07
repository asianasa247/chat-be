using System.Text;

namespace ManageEmployee.Helpers;

public static class MD5Ultility
{
    public static string saltKey = "Jo93Ciipi2uK5oA2M2Sx3FP9ynkfcDpaTVpT3qyxWTqYAr9UNydZNvfeFVaX4yS9ceKqxeNCdQCzFQX7MtaSpifMyZK7n3PVoyRCXbW27QrKaqJKNPyYgm3N2yqaMT5oppJjQSy3vcJrvAQPT3zv9K4PiAPpYXtCivMNYmsjaRvqcEsVrF3XmbcfQLUdprKMEdJdtW3WjpioLRgX2oArMhTtLx23XKMzRVnnh2cYvtFQcm5yqh75kHbgtuuw2v9y";
    /// <summary>
    /// Mã hóa mật khẩu
    /// </summary>
    /// <param name="password"></param>
    /// <param name="salt"></param>
    /// <returns></returns>
    public static string HashPassword(string password, string salt)
    {
        byte[] data = Encoding.ASCII.GetBytes(salt + password);
        data = System.Security.Cryptography.MD5.Create().ComputeHash(data);
        return Convert.ToBase64String(data);
    }
}
