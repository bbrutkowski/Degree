using Degree.Models.DTO;
using Degree.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Degree.Controllers
{
    [Route("[controller]")]
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService) => _shoppingCartService = shoppingCartService;

        public IActionResult Index()
        {
            var cart = _shoppingCartService.GetCart();
            return View(cart.Value);
        }

        [HttpPost("Add")]
        public IActionResult AddToCart([FromBody] CartItemDto product)
        {
            if (product is null) return NotFound();

            var addingResult = _shoppingCartService.AddItemToCart(product);
            if (addingResult.IsFailure) return BadRequest();

            var cart = _shoppingCartService.GetCart();

            return Ok(cart.Value.Count);
        }
    }
}
