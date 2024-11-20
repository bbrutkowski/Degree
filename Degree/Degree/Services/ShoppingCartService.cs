using CSharpFunctionalExtensions;
using Degree.Models;
using Degree.Models.DTO;
using Degree.Services.Interfaces;

namespace Degree.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ISessionService _sessionService;
        private readonly ILogger<ShoppingCartService> _logger;

        public ShoppingCartService(ISessionService sessionService, ILogger<ShoppingCartService> logger)
        {
            _sessionService = sessionService;
            _logger = logger;
        }

        public Result<List<CartItem>> GetCart()
        {
            _logger.LogInformation("Getting shopping cart from session");

            try
            {
                var cart = _sessionService.GetObjectFromJson<List<CartItem>>("Cart") ?? [];

                return Result.Success(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting shopping cart: {ex.Message}");
                return Result.Failure<List<CartItem>>(ex.Message);
            }
        }

        public Result AddItemToCart(CartItemDto cartItem)
        {
            _logger.LogInformation("Adding product to the cart");

            try
            {
                var cart = GetCart().Value;
                var existingItem = cart.FirstOrDefault(c => c.Item.ProductId == cartItem.ProductId);

                if (existingItem is not null)
                {
                    _logger.LogInformation("The amount of the product will be increased");
                    existingItem.Amount++;
                }
                else
                {
                    cart.Add(new CartItem { Item = cartItem, Amount = 1 });                  
                }

                _sessionService.SetObjectAsJson("Cart", cart);

                _logger.LogInformation($"Product added to cart: {cartItem.Title}");

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding product to cart: {ex.Message}");
                return Result.Failure(ex.Message);
            }
        }
    }
}
