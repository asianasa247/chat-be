using HtmlAgilityPack;
using ManageEmployee.Entities.Enumerations;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ManageEmployee.Helpers;

public static class StringHelpers
{
    public static string GetStringWithMaxLength(string input, int maxLength)
    {
        try
        {
            if(!string.IsNullOrEmpty(input) && input.Length > maxLength)
            {
                    return input.Substring(0, maxLength) + " ... ";
            }
            return input;
        }
        catch
        {
            return input;
        }
    }
    public static string GenderVI(GenderEnum gender)
    {
        switch (gender)
        {
            case GenderEnum.All:
                return "Tất cả";
            case GenderEnum.Male:
                return "Nam";
            case GenderEnum.Female:
                return "Nữ";
            case GenderEnum.Other:
                return "Khác";
            default:
                return "";
        }
    }
    public static string GenerateCodeVerifier()
    {
        const int codeVerifierLength = 64; // Có thể là 43-128 ký tự
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] randomBytes = new byte[codeVerifierLength];
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "-")  // Base64 URL Safe Encoding
                .Replace("/", "_")
                .Replace("=", ""); // Loại bỏ padding
        }
    }

    public static string GenerateCodeChallenge(string codeVerifier)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
            return Convert.ToBase64String(hash)
                .Replace("+", "-")  // Base64 URL Safe Encoding
                .Replace("/", "_")
                .Replace("=", ""); // Loại bỏ padding
        }
    }
    public static string Decrypt(this string cipherString, bool useHashing = true)
    {
        try
        {
            byte[] keyArray;
            //get the byte code of the string
            cipherString = cipherString.Trim().Replace(" ", "+");
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);


            //Get your key from config file to open the lock!
            string key = "fZOictFPYijxHjwKwyJKqhTb3FtpHagB";
            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        catch
        {
            return null;
        }
    }

    public static string Encrypt(this string toEncrypt, bool useHashing = true)
    {
        byte[] keyArray;
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);



        string key = "fZOictFPYijxHjwKwyJKqhTb3FtpHagB";
        //System.Windows.Forms.MessageBox.Show(key);
        //If hashing use get hashcode regards to your key
        if (useHashing)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            //Always release the resources and flush data
            // of the Cryptographic service provide. Best Practice

            hashmd5.Clear();
        }
        else
            keyArray = UTF8Encoding.UTF8.GetBytes(key);

        TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
        //set the secret key for the tripleDES algorithm
        tdes.Key = keyArray;
        //mode of operation. there are other 4 modes.
        //We choose ECB(Electronic code Book)
        tdes.Mode = CipherMode.ECB;
        //padding mode(if any extra byte added)

        tdes.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = tdes.CreateEncryptor();
        //transform the specified region of bytes array to resultArray
        byte[] resultArray =
          cTransform.TransformFinalBlock(toEncryptArray, 0,
          toEncryptArray.Length);
        //Release resources held by TripleDes Encryptor
        tdes.Clear();
        //Return the encrypted data into unreadable string format
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }
    public static string StripHtml(this string html)
    {
        if (string.IsNullOrEmpty(html))
            return "";
        html = Regex.Replace(html, @"<!DOCTYPE[^>]*>", "", RegexOptions.IgnoreCase);
        string decodedHtml = HttpUtility.HtmlDecode(html);
        decodedHtml = Regex.Replace(decodedHtml, @"<!--(.*?)-->", "", RegexOptions.Singleline);
        var doc = new HtmlDocument();
        doc.LoadHtml(decodedHtml);

        // Xóa <style> và <script>
        foreach (var node in doc.DocumentNode.SelectNodes("//style|//script") ?? Enumerable.Empty<HtmlNode>())
            node.Remove();

        // Lấy nội dung chỉ chứa text
        string text = doc.DocumentNode.InnerText;

        // Xóa các khoảng trắng thừa
        text = Regex.Replace(text, @"\s+", " ").Trim();
        text = text.Replace("Emails/html", "").Trim();
        return text;
    }
}
