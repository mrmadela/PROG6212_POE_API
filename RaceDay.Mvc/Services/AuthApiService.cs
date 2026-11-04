using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using RaceDay.Contracts;

namespace RaceDay.Mvc.Services
{// start of namespace
    // start of class
    public class AuthApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthApiService(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        // start of RegisterAsync method
        public async Task<AuthResponse?> RegisterAsync(
            RegisterRequest request)
        {
            var parameters = new Dictionary<string, string?>
            {
                ["email"] = request.Email,
                ["password"] = request.Password,
                ["role"] = request.Role,
                ["fullName"] = request.FullName,
                ["phoneNum"] = request.PhoneNum,
                ["idNum"] = request.IdNum,
                ["address"] = request.Address
            };

            string url = QueryHelpers.AddQueryString(
                "api/auth/register", parameters);

            HttpResponseMessage response =
                await _httpClient.PostAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Registration failed: {error}");
            }

            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }
        // end of RegisterAsync method

        // start of LoginAsync method
        public async Task<AuthResponse?> LoginAsync(
            LoginRequest request)
        {
            var parameters = new Dictionary<string, string?>
            {
                ["email"] = request.Email,
                ["password"] = request.Password
            };

            string url = QueryHelpers.AddQueryString(
                "api/auth/login", parameters);

            HttpResponseMessage response =
                await _httpClient.PostAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    "Invalid email or password.");
            }

            // Read the API response.
            AuthResponse? authResponse =
                await response.Content.ReadFromJsonAsync<AuthResponse>();

            // Capture the API session cookie.
            if (response.Headers.TryGetValues(
                "Set-Cookie", out var cookieHeaders))
            {
                string? sessionCookie = cookieHeaders
                    .FirstOrDefault(cookie =>
                        cookie.StartsWith(
                            ".RaceDay.Session=",
                            StringComparison.OrdinalIgnoreCase));

                if (sessionCookie != null)
                {
                    string cookieValue =
                        sessionCookie.Split(';')[0];

                    _httpContextAccessor.HttpContext?
                        .Session.SetString(
                            "ApiSessionCookie", cookieValue);
                }
            }

            return authResponse;
        }
        // end of LoginAsync method

        // start of LogoutAsync method
        public async Task LogoutAsync()
        {
            var httpContext =
                _httpContextAccessor.HttpContext;

            string? apiCookie =
                httpContext?.Session.GetString("ApiSessionCookie");

            if (!string.IsNullOrWhiteSpace(apiCookie))
            {
                using var request = new HttpRequestMessage(
                    HttpMethod.Post, "api/auth/logout");

                request.Headers.Add("Cookie", apiCookie);

                await _httpClient.SendAsync(request);
            }

            // Clear the MVC session.
            httpContext?.Session.Clear();
        }
        // end of LogoutAsync method
    }
    // end of class
}// end of namespace
