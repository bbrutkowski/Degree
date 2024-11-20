using CSharpFunctionalExtensions;
using Degree.Models;
using Degree.Models.DTO;

namespace Degree.Services.Interfaces
{
    public interface IProductService
    {
        Task<Result<IReadOnlyCollection<ProductDto>>> GetProductsAsync(CancellationToken token);
        Result<PageItemsDto> CalculatePageItems(IReadOnlyCollection<ProductDto> products, int page);
    }
}
