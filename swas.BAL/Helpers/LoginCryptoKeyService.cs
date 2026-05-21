using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace swas.BAL.Helpers
{
    public class LoginCryptoKeyService
    {

        private const string LoginCryptoKeySessionName = "LOGIN_CRYPTO_KEY";

        public string EnsureLoginCryptoKey(HttpContext httpContext)
        {
            var existingKey = httpContext.Session.GetString(LoginCryptoKeySessionName);

            if (!string.IsNullOrWhiteSpace(existingKey))
            {
                return existingKey;
            }

            var newKey = GenerateRandomAes256Key();

            httpContext.Session.SetString(LoginCryptoKeySessionName, newKey);

            return newKey;
        }

        public string? GetLoginCryptoKey(HttpContext httpContext)
        {
            return httpContext.Session.GetString(LoginCryptoKeySessionName);
        }

        public void RemoveLoginCryptoKey(HttpContext httpContext)
        {
            httpContext.Session.Remove(LoginCryptoKeySessionName);
        }

        private static string GenerateRandomAes256Key()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            var bytes = new byte[32];
            RandomNumberGenerator.Fill(bytes);

            var result = new char[32];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = chars[bytes[i] % chars.Length];
            }

            return new string(result);
        }
    }
}
