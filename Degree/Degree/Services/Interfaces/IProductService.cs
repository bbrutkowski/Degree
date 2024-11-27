using CSharpFunctionalExtensions;
using Degree.Models;
using Degree.Models.DTO;

namespace Degree.Services.Interfaces
{
    public interface IProductService
    {
        Task<Result<IReadOnlyCollection<ProductDto>>> GetProductsAsync(CancellationToken token);
        Result<PageItemsDto<T>> CalculatePageItems<T>(IReadOnlyCollection<T> source, int page);
        Task<Result<List<string>>> GetCategoriesAsync(CancellationToken token);
        Task<Result<IReadOnlyCollection<ProductDto>>> GetProductsByCategoryAsync(string category, CancellationToken token);
    }
}
