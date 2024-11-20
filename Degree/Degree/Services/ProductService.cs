using CSharpFunctionalExtensions;
using Degree.Models;
using Degree.Models.DTO;
using Degree.Services.Interfaces;
using Newtonsoft.Json;

namespace Degree.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpService _httpService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILogger<ProductService> _logger;

        private const string _productsUrl = "https://fakestoreapi.com/products";
        private const string _deserializationError = "Error while products deserialization";

        public ProductService(IHttpService httpService,
                              IShoppingCartService shoppingCartService,
                              ILogger<ProductService> logger)
        {
            _httpService = httpService;
            _shoppingCartService = shoppingCartService;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyCollection<ProductDto>>> GetProductsAsync(CancellationToken token)
        {
            _logger.LogInformation("Downloading products");

            var createRequestResult = _httpService.CreateGetRequest(_productsUrl);
            if (createRequestResult.IsFailure) return Result.Failure<IReadOnlyCollection<ProductDto>>(createRequestResult.Error);

            var sendRequestResult = await _httpService.SendGetRequestAsync(createRequestResult.Value, token);
            if (sendRequestResult.IsFailure) return Result.Failure<IReadOnlyCollection<ProductDto>>(sendRequestResult.Error);

            var convertResult = ConvertResponseToProducts(sendRequestResult.Value);
            if (convertResult.IsFailure) return Result.Failure<IReadOnlyCollection<ProductDto>>(convertResult.Error);

            _logger.LogInformation("Download completed successfully");

            return Result.Success(convertResult.Value);
        }

        private Result<IReadOnlyCollection<ProductDto>> ConvertResponseToProducts(string response)
        {
            try
            {
                var products = JsonConvert.DeserializeObject<IReadOnlyCollection<ProductDto>>(response);
                if (products is null) return Result.Failure<IReadOnlyCollection<ProductDto>>(_deserializationError);

                _logger.LogInformation("Deserialization complete");

                return Result.Success(products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{_deserializationError}: {ex.Message}");
                return Result.Failure<IReadOnlyCollection<ProductDto>>(ex.Message);
            }
        }

        public Result<PageItemsDto> CalculatePageItems(IReadOnlyCollection<ProductDto> products, int page)
        {
            _logger.LogInformation("Calculating products per page");

            var pageSize = 10;

            try
            {
                var paginatedProducts = products
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var cartResult = _shoppingCartService.GetCart();
                var cart = cartResult.Value;
                var totalPages = (int)Math.Ceiling((double)products.Count / pageSize);

                var pageItems = new PageItemsDto
                {
                    CartItemCount = cart.Count,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    Products = paginatedProducts
                };

                return Result.Success(pageItems);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while calculating products: {ex.Message}");
                return Result.Failure<PageItemsDto>(ex.Message);
            }
        }
    }
}
