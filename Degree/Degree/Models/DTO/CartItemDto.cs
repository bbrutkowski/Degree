namespace Degree.Models.DTO
{
    public record CartItemDto
    {
        public int ProductId { get; set; }
        public string? Title { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
    }
}
