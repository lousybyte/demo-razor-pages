using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DemoRazor.Contexts;
using DemoRazor.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DemoRazor.Services
{
    public class CaptchaVerificationService
    {
        private readonly ILogger<CaptchaVerificationService> Logger;
        private readonly IConfiguration                      Config;
        private readonly AppDbContext                        AppDb;
        private readonly CaptchaSettings                     CaptchaSettings;

        public CaptchaVerificationService(
            ILogger<CaptchaVerificationService> logger,
            IConfiguration config, AppDbContext appDbContext,
            IOptionsSnapshot<CaptchaSettings>   captchaSettings)
        {
            Logger          = logger;
            Config          = config;
            AppDb           = appDbContext;
            CaptchaSettings = captchaSettings.Value;
        }

        public async Task<bool> IsCaptchaValid(string token)
        {
            var result = false;
            const string googleVerificationUrl = "https://www.google.com/recaptcha/api/siteverify";

            try
            {
                using var client = new HttpClient();

                var response = await client.PostAsync($"{googleVerificationUrl}?secret={Environment.GetEnvironmentVariable("CAPTCHA_SERVER_KEY")}&response={token}", null);
                var jsonString = await response.Content.ReadAsStringAsync();
                var captchaVerfication = JsonSerializer.Deserialize<CaptchaVerificationResponse>(jsonString);

                result = captchaVerfication.Success;
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to process captcha validation", e);
            }

            return result;
        }

        public class CaptchaVerificationResponse
        {
            public bool     Success { get; set; }

            [JsonPropertyName("challenge_ts")]
            public DateTime ChallengeTimestamp { get; set; }

            public string   Hostname { get; set; }

            [JsonPropertyName("error-codes")]
            public string[] Errorcodes { get; set; }
        }
    }
}
