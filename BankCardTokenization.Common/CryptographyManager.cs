using System;
using System.Security.Cryptography;
using System.Text;

namespace BankCardTokenization.Common
{
    public static class CryptographyManager
    {
        public static string GenerateSalt()
        {
            var rngCsp = new RNGCryptoServiceProvider();
            var buff = new byte[32];
            rngCsp.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

        public static string GenerateSHA256Hash(string pass, string salt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(pass + salt);
            var sha256Hasher = new SHA256Managed();

            var hash = sha256Hasher.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }
}
