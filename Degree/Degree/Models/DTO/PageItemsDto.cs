using Degree.Models.DTO;

namespace Degree.Models
{
    public record PageItemsDto
    {
        public int CartItemCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<ProductDto> Products { get; set; } = new();
    }
}
