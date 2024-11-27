using CSharpFunctionalExtensions;
using Degree.Models;
using Degree.Models.DTO;
using Degree.Services.Interfaces;

namespace Degree.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpService _httpService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPaginationService _paginationService; 
        private readonly ILogger<ProductService> _logger;

        public ProductService(IHttpService httpService,
                              IShoppingCartService shoppingCartService,
                              IPaginationService paginationService,
                              ILogger<ProductService> logger)
        {
            _httpService = httpService;
            _shoppingCartService = shoppingCartService;
            _paginationService = paginationService;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyCollection<ProductDto>>> GetProductsAsync(CancellationToken token)
        {
            var fetchProductsResult = await _httpService.GetProductsAsync(token);
            if (fetchProductsResult.IsFailure) return Result.Failure<IReadOnlyCollection<ProductDto>>(fetchProductsResult.Error);

            _logger.LogInformation("Fetching completed successfully");

            return Result.Success(fetchProductsResult.Value);
        }

        public async Task<Result<List<string>>> GetCategoriesAsync(CancellationToken token)
        {
            var categoriesResult = await _httpService.GetCategoriesAsync(token);
            if (categoriesResult.IsFailure) return Result.Failure<List<string>> (categoriesResult.Error);

            _logger.LogInformation("Fetching categories completed");

            return Result.Success(categoriesResult.Value);
        }

        public async Task<Result<IReadOnlyCollection<ProductDto>>> GetProductsByCategoryAsync(string category, CancellationToken token)
        {
            var categoriesResult = await _httpService.GetProductsByCategoryAsync(category, token);
            if (categoriesResult.IsFailure) return Result.Failure<IReadOnlyCollection<ProductDto>>(categoriesResult.Error);

            _logger.LogInformation("Fetching products by category completed");

            return Result.Success(categoriesResult.Value);
        }

        public Result<PageItemsDto<T>> CalculatePageItems<T>(IReadOnlyCollection<T> source, int page)
        {
            _logger.LogInformation("Calculating products per page");
            var pageSize = 10;

            try
            {
                var paginatedResult = _paginationService.Paginate(source, page, pageSize);
                if (paginatedResult.IsFailure) return Result.Failure<PageItemsDto<T>>(paginatedResult.Error);

                var cartItemCount = GetCartItemCount();

                var pageItems = new PageItemsDto<T>
                {
                    CartItemCount = cartItemCount,
                    CurrentPage = paginatedResult.Value.CurrentPage,
                    TotalPages = paginatedResult.Value.TotalPages,
                    Items = paginatedResult.Value.Items
                };

                return Result.Success(pageItems);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while calculating products: {ex.Message}");
                return Result.Failure<PageItemsDto<T>>(ex.Message);
            }
        }

        private int GetCartItemCount()
        {
            var cartResult = _shoppingCartService.GetCart();
            if (cartResult.IsFailure)
            {
                _logger.LogWarning("Failed to retrieve cart data.");
                return 0;
            }

            return cartResult.Value.Count;
        }
    }
}
