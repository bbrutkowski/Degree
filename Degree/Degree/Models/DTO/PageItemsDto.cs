using Degree.Models.DTO;

namespace Degree.Models
{
    public record PageItemsDto<T>
    {
        public int CartItemCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyCollection<T>? Items { get; set; }
    }
}
