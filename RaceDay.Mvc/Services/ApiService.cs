
using System.Net.Http.Json;

namespace RaceDay.Mvc.Services
{// start of namespace

    public class ApiService : IApiService
    {// start of class

        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor)
        {// start of constructor

            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;

        }// end of constructor

        // Adds the current user's API session cookie to the request.
        private HttpRequestMessage CreateRequest(
            HttpMethod method,
            string endpoint)
        {// start of CreateRequest

            var request = new HttpRequestMessage(method, endpoint);

            var apiSessionCookie = _httpContextAccessor
                .HttpContext?
                .Session
                .GetString("ApiSessionCookie");

            if (!string.IsNullOrWhiteSpace(apiSessionCookie))
            {
                request.Headers.Add("Cookie", apiSessionCookie);
            }

            return request;

        }// end of CreateRequest

        // GET: Retrieve information from the API.
        public async Task<TResponse?> GetAsync<TResponse>(
            string endpoint)
        {// start of GetAsync

            using var request = CreateRequest(
                HttpMethod.Get,
                endpoint);

            using var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<TResponse>();

        }// end of GetAsync

        // POST: Create a new record through the API.
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(
            string endpoint,
            TRequest requestData)
        {// start of PostAsync

            using var request = CreateRequest(
                HttpMethod.Post,
                endpoint);

            request.Content = JsonContent.Create(requestData);

            using var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<TResponse>();

        }// end of PostAsync

        // PUT: Update an existing record through the API.
        public async Task<TResponse?> PutAsync<TRequest, TResponse>(
            string endpoint,
            TRequest requestData)
        {// start of PutAsync

            using var request = CreateRequest(
                HttpMethod.Put,
                endpoint);

            request.Content = JsonContent.Create(requestData);

            using var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<TResponse>();

        }// end of PutAsync

        // DELETE: Returns true when the API successfully deletes the record.
        public async Task<bool> DeleteAsync(string endpoint)
        {// start of DeleteAsync

            using var request = CreateRequest(
                HttpMethod.Delete,
                endpoint);

            using var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;

        }// end of DeleteAsync

    }// end of class

}// end of namespace
