namespace RaceDay.Mvc.Services
{// start of namespace
    // start of interface
    public interface IApiService
    {
        Task<TResponse?> GetAsync<TResponse>(string endpoint);

        Task<TResponse?> PostAsync<TRequest, TResponse>(
            string endpoint, TRequest request);

        Task<TResponse?> PutAsync<TRequest, TResponse>(
            string endpoint, TRequest request);

        Task<bool> DeleteAsync(string endpoint);
    }
    // end of interface
}// end of namespace
