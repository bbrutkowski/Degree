using Degree.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Degree.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService) => _productService = productService;

        public async Task<IActionResult> Index(CancellationToken token, int page = 1)
        {
            var products = await _productService.GetProductsAsync(token);
            if (!products.Value.Any()) return NoContent();

            var pageItemsResult = _productService.CalculatePageItems(products.Value, page);
            if (pageItemsResult.IsFailure) return NotFound();

            ViewBag.CartItemCount = pageItemsResult.Value.CartItemCount;
            ViewBag.CurrentPage = pageItemsResult.Value.CurrentPage;
            ViewBag.TotalPages = pageItemsResult.Value.TotalPages;

            return View(pageItemsResult.Value.Products);
        }
    }
}
