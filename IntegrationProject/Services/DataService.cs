namespace IntegrationProject.Services
{
    public class DataService
    {
        private readonly HttpClient _httpClient;

        public DataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        internal async Task<HttpResponseMessage> GetRawCsv(string request)
        {
            return await _httpClient.GetAsync(request);
        }
    }
}
