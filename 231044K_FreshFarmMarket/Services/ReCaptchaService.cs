using _231044K_FreshFarmMarket.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace _231044K_FreshFarmMarket.Services
{
    public class ReCaptchaService : IReCaptchaService
    {
        private readonly HttpClient _httpClient;
        private readonly GoogleReCaptchaOptions _options;

        public ReCaptchaService(HttpClient httpClient, IOptions<GoogleReCaptchaOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        // ✅ Main method
        public async Task<bool> VerifyTokenAsync(string token, string action)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var url =
                "https://www.google.com/recaptcha/api/siteverify" +
                $"?secret={_options.SecretKey}&response={token}";

            var response = await _httpClient.PostAsync(url, null);
            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ReCaptchaResponse>(json);

            return result != null
                   && result.success
                   && string.Equals(result.action ?? "", action, StringComparison.OrdinalIgnoreCase)
                   && result.score >= 0.5f;
        }

        // ✅ Alias method for older code that calls VerifyAsync
        public Task<bool> VerifyAsync(string token, string action)
            => VerifyTokenAsync(token, action);

        private class ReCaptchaResponse
        {
            public bool success { get; set; }
            public float score { get; set; }
            public string? action { get; set; }
        }
    }
}
