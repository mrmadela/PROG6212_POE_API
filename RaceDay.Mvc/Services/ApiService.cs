using System.Net.Http.Json;

namespace RaceDay.Mvc.Services
{// start of namespace
    // start of class
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        // start of constructor
        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        // end of constructor

        // start of GetAsync method
        public async Task<TResponse?> GetAsync<TResponse>(
            string endpoint)
        {
            return await _httpClient.GetFromJsonAsync<TResponse>(
                endpoint);
        }
        // end of GetAsync method

        // start of PostAsync method
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(
            string endpoint, TRequest request)
        {
            HttpResponseMessage response =
                await _httpClient.PostAsJsonAsync(endpoint, request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
        // end of PostAsync method

        // start of PutAsync method
        public async Task<TResponse?> PutAsync<TRequest, TResponse>(
            string endpoint, TRequest request)
        {
            HttpResponseMessage response =
                await _httpClient.PutAsJsonAsync(endpoint, request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
        // end of PutAsync method

        // start of DeleteAsync method
        public async Task<bool> DeleteAsync(string endpoint)
        {
            HttpResponseMessage response =
                await _httpClient.DeleteAsync(endpoint);

            return response.IsSuccessStatusCode;
        }
        // end of DeleteAsync method
    }
    // end of class
}// end of namespace
