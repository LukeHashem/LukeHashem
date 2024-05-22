using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MorseCodeTranslator
{

    public class AESEncryption
    {
        private readonly byte[] _salt;

        private readonly string _password;

        public AESEncryption(string password)
        {
            _salt = Encoding.UTF8.GetBytes("Random");
            _password = password;
        }

        public string Encrypt(string text)
        {
            byte[] encryptedBytes;

            using (Aes aes = Aes.Create())
            {
                var key = new Rfc2898DeriveBytes(_password, _salt, 1000).GetBytes(32);
                var iv = new Rfc2898DeriveBytes(_password, _salt, 1000).GetBytes(16);

                aes.Key = key;
                aes.IV = iv;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                var plainTextBytes = Encoding.UTF8.GetBytes(text);

                encryptedBytes = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        public string Decrypt(string text)
        {
            try
            {
                byte[] decryptedBytes;

                using (Aes aes = Aes.Create())
                {
                    var key = new Rfc2898DeriveBytes(_password, _salt, 1000).GetBytes(32);
                    var iv = new Rfc2898DeriveBytes(_password, _salt, 1000).GetBytes(16);

                    aes.Key = key;
                    aes.IV = iv;

                    var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    var plainTextBytes = Convert.FromBase64String(text);

                    decryptedBytes = decryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);

                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
            catch
            {
                Console.WriteLine("Error incorrect key returning to main menu.");
                Console.WriteLine("");

                return "Error";
            }
        }
    }
}
