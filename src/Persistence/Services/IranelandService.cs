using Application.Contracts.Persistence.Services;
using Domain;
using Domain.Dtos.ExternalAPI;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Persistence.Services
{
    public class IranelandService : IIranelandService
    {
        private readonly IMemoryCache _memoryCache;

        public IranelandService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<CasesAndUsersResponse> GetCasesAndUsersAsync(CancellationToken cancellationToken = default)
        {
            CasesAndUsersResponse data = null;
            try
            {
                if (_memoryCache.TryGetValue("IranelandCases", out data))
                    return data;

                byte[] iv = Encoding.UTF8.GetBytes(SD.IranelandIV);

                var date = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + DateTime.Now.AddHours(1).Hour + "1994";
                var dateNumber = Convert.ToInt64(date) * 11;

                byte[] cryptKey = SHA256.HashData(Encoding.UTF8.GetBytes(SD.IranelandSecretKey));
                string clearData = dateNumber.ToString();

                string encryptedData = IranelandEncryptionUtil.Encode(cryptKey, iv, clearData);

                using var client = new HttpClient();
                var values = new Dictionary<string, string> { { "token", encryptedData } };
                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync($"https://iraneland.ir/ow/service/ow/portalStatisticService", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Console.WriteLine(responseString);
                    var jsonData = IranelandEncryptionUtil.Decode(cryptKey, iv, responseString);

                    data = JsonSerializer.Deserialize<CasesAndUsersResponse>(jsonData);
                    _memoryCache.Set("IranelandCases", data, DateTimeOffset.Now.AddHours(1));

                    return data;

                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }

    public class IranelandEncryptionUtil
    {
        public static string Decode(byte[] cryptKey, byte[] iv, string secretData)
        {
            using (AesManaged aes = new AesManaged())
            {
                aes.Key = cryptKey;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                byte[] encryptedData = Convert.FromBase64String(secretData);

                string decryptedData;
                using (MemoryStream ms = new MemoryStream(encryptedData))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                        {
                            decryptedData = reader.ReadToEnd();
                        }
                    }
                }

                return decryptedData;
            }
        }

        public static string Encode(byte[] cryptKey, byte[] iv, string clearData)
        {
            using (AesManaged aes = new AesManaged())
            {
                aes.Key = cryptKey;
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                byte[] clearBytes = Encoding.UTF8.GetBytes(clearData);

                byte[] encryptedData;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                        encryptedData = ms.ToArray();
                    }
                }

                return Convert.ToBase64String(encryptedData);
            }
        }

        public static string Base64Encode(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(bytes);
        }
    }

}
