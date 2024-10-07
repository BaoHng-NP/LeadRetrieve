using LeadRetrieve.Models;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace LeadRetrieve
{
    public class PageTokenService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PageTokenService> _logger; // Thêm ILogger
        private string _pageToken;
        private DateTime _tokenExpiration;

        public PageTokenService(HttpClient httpClient, ILogger<PageTokenService> logger) // Tiêm ILogger
        {
            _httpClient = httpClient;
            _logger = logger; 
            _pageToken = null;
            _tokenExpiration = DateTime.MinValue;
        }

        // GET page token api
        public async Task<string> GetPageTokenAsync()
        {
            if (_pageToken != null && _tokenExpiration > DateTime.UtcNow)
            {
                return _pageToken;
            }

            string accessToken = Environment.GetEnvironmentVariable("USER_ACCESS_TOKEN");
            string appId = Environment.GetEnvironmentVariable("APP_ID");
            string appSecret = Environment.GetEnvironmentVariable("APP_SECRET");



            string requestUrl = $"https://graph.facebook.com/v20.0/oauth/access_token?grant_type=fb_exchange_token&client_id={appId}&client_secret={appSecret}&fb_exchange_token={accessToken}";

            using (var response = await _httpClient.GetAsync(requestUrl))
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(content);

                    _pageToken = tokenResponse.AccessToken;
                    _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

                    return _pageToken;
                }
                else
                {
                    throw new Exception("Failed to retrieve page token.");
                }
            }
        }
    }
}
