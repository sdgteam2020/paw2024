using Newtonsoft.Json;
using static swas.UI.Helpers.Helper;
using swas.BAL.Helpers;
using swas.UI.Models;

namespace swas.UI.Helpers
{
    public static class DecryptionHelper
    {
        public static (bool Success, DecryptedRequest? Data, string? ErrorMessage)
            DecryptRequest(string encryptedData, string cryptoKey, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(encryptedData))
            {
                logger.LogWarning("Encrypted data is empty");
                return (false, null, "Invalid request.");
            }

            try
            {
                string decryptedJson = CryptoHelper.SafeDecrypt(encryptedData, cryptoKey);

                if (string.IsNullOrWhiteSpace(decryptedJson))
                {
                    logger.LogWarning("Decryption returned empty result");
                    return (false, null, "Invalid request data.");
                }

                var obj = JsonConvert.DeserializeObject<DecryptedRequest>(decryptedJson.Trim('"'));

               

                return (true, obj, null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error decrypting data");
                return (false, null, "Error processing request.");
            }
        }
    }
}
