using CSharpFunctionalExtensions;
using Degree.Models.DTO;
using Degree.Services.Interfaces;
using System.Text.Json;

namespace Degree.Services
{
    public class HttpService : IHttpService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpService> _logger;

        private const string _clientName = "FakeStoreApiClient";

        public HttpService(IHttpClientFactory httpClientFactory, ILogger<HttpService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<Result<List<string>>> GetCategoriesAsync(CancellationToken token)
        {
            _logger.LogInformation("Fetching available product categories");
            var endpoint = "products/categories";

            try
            {
                var client = _httpClientFactory.CreateClient("FakeStoreApiClient");

                var response = await client.GetAsync(endpoint, token);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to fetch categories: {response.StatusCode}");
                    return Result.Failure<List<string>>($"Error: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync(token);
                var categories = JsonSerializer.Deserialize<List<string>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return Result.Success(categories ?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching categories: {ex.Message}");
                return Result.Failure<List<string>>(ex.Message);
            }
        }

        public async Task<Result<IReadOnlyCollection<ProductDto>>> GetProductsAsync(CancellationToken token)
        {
            _logger.LogInformation("Fetching all products");
            var endpoint = "products";

            try
            {
                var client = _httpClientFactory.CreateClient(_clientName);

                var response = await client.GetAsync(endpoint, token);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to fetch products: {response.StatusCode}");
                    return Result.Failure<IReadOnlyCollection<ProductDto>>($"Error: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync(token);
                var products = JsonSerializer.Deserialize<IReadOnlyCollection<ProductDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return Result.Success(products ?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching products: {ex.Message}");
                return Result.Failure<IReadOnlyCollection<ProductDto>>(ex.Message);
            }
        }

        public async Task<Result<IReadOnlyCollection<ProductDto>>> GetProductsByCategoryAsync(string category, CancellationToken token)
        {
            _logger.LogInformation("Fetching products by category");
            var endpoint = $"products/category/{category}";

            try
            {
                var client = _httpClientFactory.CreateClient(_clientName);

                var response = await client.GetAsync(endpoint, token);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to fetch products by category: {response.StatusCode}");
                    return Result.Failure<IReadOnlyCollection<ProductDto>>($"Error: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync(token);
                var products = JsonSerializer.Deserialize<IReadOnlyCollection<ProductDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return Result.Success(products ?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching products by category: {ex.Message}");
                return Result.Failure<IReadOnlyCollection<ProductDto>>(ex.Message);
            }
        }
    }
}
