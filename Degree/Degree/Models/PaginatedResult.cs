namespace Degree.Models
{
    public class PaginatedResult<T>
    {
        public List<T> Items { get; init; } = new();
        public int CurrentPage { get; init; }
        public int TotalPages { get; init; }
        public int TotalItems { get; init; }
        public int PageSize { get; init; }
    }
}
