using Degree.Models.DTO;

namespace Degree.Models
{
    public class CartItem
    {
        public CartItemDto? Item { get; set; }
        public int Amount { get; set; }
    }
}
