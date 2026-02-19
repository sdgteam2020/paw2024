using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.Helpers
{
    public static class CryptoHelper
    {
        private const string CryptoJsPrefix = "U2FsdGVkX1"; // Base64 of "Salted__"
        public static bool IsCryptoJs(string value)
        {
            return !string.IsNullOrWhiteSpace(value)
                   && value.StartsWith(CryptoJsPrefix, StringComparison.Ordinal);
        }
        public static string SafeDecrypt(string value, string passphrase)
        {
            if (!IsCryptoJs(value) || string.IsNullOrEmpty(passphrase))
                return value;

            try
            {
                return Decrypt(value, passphrase);
            }
            catch
            {
                return value;
            }
        }
        private static string Decrypt(string cipherText, string passphrase)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            if (cipherBytes.Length < 16 ||
                Encoding.ASCII.GetString(cipherBytes, 0, 8) != "Salted__")
            {
                throw new CryptographicException("Invalid CryptoJS AES format.");
            }

            byte[] salt = new byte[8];
            Buffer.BlockCopy(cipherBytes, 8, salt, 0, 8);

            DeriveKeyAndIV(passphrase, salt, out byte[] key, out byte[] iv);

            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = iv;

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);

            cs.Write(cipherBytes, 16, cipherBytes.Length - 16);
            cs.FlushFinalBlock();

            return Encoding.UTF8.GetString(ms.ToArray());
        }
        private static void DeriveKeyAndIV(
            string passphrase,
            byte[] salt,
            out byte[] key,
            out byte[] iv)
        {
            using var md5 = MD5.Create();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(passphrase);

            byte[] hash = Array.Empty<byte>();
            byte[] result = Array.Empty<byte>();

            while (result.Length < 48)
            {
                byte[] input = new byte[hash.Length + passwordBytes.Length + salt.Length];

                Buffer.BlockCopy(hash, 0, input, 0, hash.Length);
                Buffer.BlockCopy(passwordBytes, 0, input, hash.Length, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, input, hash.Length + passwordBytes.Length, salt.Length);

                hash = md5.ComputeHash(input);
                result = Combine(result, hash);
            }

            key = new byte[32]; // 256-bit key
            iv = new byte[16];  // 128-bit IV

            Buffer.BlockCopy(result, 0, key, 0, 32);
            Buffer.BlockCopy(result, 32, iv, 0, 16);
        }

        private static byte[] Combine(byte[] a, byte[] b)
        {
            var output = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, output, 0, a.Length);
            Buffer.BlockCopy(b, 0, output, a.Length, b.Length);
            return output;
        }
    }
}
