using CSharpFunctionalExtensions;
using Degree.Models.DTO;

namespace Degree.Services.Interfaces
{
    public interface IHttpService
    {
        Task<Result<IReadOnlyCollection<ProductDto>>> GetProductsAsync(CancellationToken token);
        Task<Result<List<string>>> GetCategoriesAsync(CancellationToken token);
        Task<Result<IReadOnlyCollection<ProductDto>>> GetProductsByCategoryAsync(string category, CancellationToken token);
    }
}
