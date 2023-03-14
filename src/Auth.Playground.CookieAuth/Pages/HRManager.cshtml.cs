using System.Net.Http.Headers;
using System.Text.Json;
using Auth.Playground.CookieAuth.Authorization;
using Auth.Playground.CookieAuth.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Playground.CookieAuth.Pages
{
    [Authorize(Policy = Constants.HrManagerOnlyPolicyName)]
    public class HRManagerModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public List<WeatherForecastDTO> WeatherForecastItems { get; set; }

        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            WeatherForecastItems = await InvokeEndpoint<List<WeatherForecastDTO>>("OurWebAPI", "WeatherForecast");
        }

        private async Task<JwtToken> Authenticate()
        {
            // authentication and getting the token
            var httpClient = _httpClientFactory.CreateClient("OurWebAPI");
            var response = await httpClient.PostAsJsonAsync("auth", new
            {
                UserName = "admin",
                Password = "password"
            });

            response.EnsureSuccessStatusCode();
            var jwtString = await response.Content.ReadAsStringAsync();
            HttpContext.Session.SetString("access_token", jwtString);
            return JsonSerializer.Deserialize<JwtToken>(jwtString);
        }

        private async Task<T> InvokeEndpoint<T>(string clientName, string url)
        {
            JwtToken jwtToken;
            var jwtString = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrWhiteSpace(jwtString))
            {
                jwtToken = await Authenticate();

            }
            else
            {
                jwtToken = JsonSerializer.Deserialize<JwtToken>(jwtString);
            }

            if (jwtToken == null
                || string.IsNullOrWhiteSpace(jwtToken.AccessToken)
                || jwtToken.ExpiresAt <= DateTime.UtcNow)
            {
                jwtToken = await Authenticate();
            }

            var httpClient = _httpClientFactory.CreateClient(clientName);
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken?.AccessToken);

            return await httpClient.GetFromJsonAsync<T>(url);
        }
    }
}
