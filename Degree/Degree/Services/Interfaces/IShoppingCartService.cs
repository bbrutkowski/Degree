using CSharpFunctionalExtensions;
using Degree.Models;
using Degree.Models.DTO;

namespace Degree.Services.Interfaces
{
    public interface IShoppingCartService
    {
        Result<List<CartItem>> GetCart();
        Result AddItemToCart(CartItemDto product);
    }
}
