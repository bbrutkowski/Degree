using CSharpFunctionalExtensions;
using Degree.Models;

namespace Degree.Services.Interfaces
{
    public interface IPaginationService
    {
        Result<PaginatedResult<T>> Paginate<T>(IEnumerable<T> source, int page, int pageSize);
    }
}
