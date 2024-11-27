using CSharpFunctionalExtensions;
using Degree.Models;
using Degree.Services.Interfaces;

namespace Degree.Services
{
    public class PaginationService : IPaginationService
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILogger<PaginationService> _logger;

        private const string _validationErrorMassage = "Pagination validation failed";

        public PaginationService(IShoppingCartService shoppingCartService, ILogger<PaginationService> logger)
        {
            _shoppingCartService = shoppingCartService;
            _logger = logger;
        }

        public Result<PaginatedResult<T>> Paginate<T>(IEnumerable<T> source, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0 || !source.Any())
            {
                _logger.LogError($"{_validationErrorMassage}. Source items count: {source.Count()}, Page value: {page}, Page size value: {pageSize}");
                return Result.Failure<PaginatedResult<T>>(_validationErrorMassage);
            }

            try
			{          
                var totalItems = source.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var items = GetPaginatedItems(source, page, pageSize);

                var paginatedResult = new PaginatedResult<T>
                {
                    Items = items,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    TotalItems = totalItems,
                    PageSize = pageSize
                };

                return Result.Success(paginatedResult);
            }
			catch (Exception ex)
			{
                _logger.LogError($"An error occurred during pagination: {ex.Message}");
                return Result.Failure<PaginatedResult<T>>(ex.Message);
			}
        }

        private List<T> GetPaginatedItems<T>(IEnumerable<T> source, int page, int pageSize)
        {
            return source
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}
