using System.Security.Cryptography;
using System.Text;

namespace ManageEmployee.Helpers
{
    public interface IAesEncryptionHelper
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }

    public class AesEncryptionHelper : IAesEncryptionHelper
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AesEncryptionHelper(IConfiguration configuration)
        {
            _key = Convert.FromBase64String(configuration["EncryptionSettings:Key"]);
            _iv = Convert.FromBase64String(configuration["EncryptionSettings:IV"]);
        }

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var cipherBytes = Convert.FromBase64String(cipherText);

            using var ms = new MemoryStream(cipherBytes);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }
    }
}
