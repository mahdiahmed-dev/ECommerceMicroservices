using Microsoft.Extensions.Options;
using OrderService.Configuration;
using OrderService.Models;

namespace OrderService.Services
{
    public class ProductApiClient : IProductApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ServiceUrls _serviceUrls;
        public ProductApiClient(HttpClient httpClient, IOptions<ServiceUrls> options)
        {
            _httpClient = httpClient;
            _serviceUrls = options.Value;
        }

        public async Task<bool> AreProductIdsValidAsync(List<Guid> productIds)
        {
            var request = new
            {
                productIds = productIds
            };

            var endpoint = _serviceUrls.ValidateProductEndpoint;
            var response = await _httpClient.PostAsJsonAsync(endpoint, request);

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<ValidateResponse>();
            return result?.IsValid ?? false;
        }
    }
    

}
