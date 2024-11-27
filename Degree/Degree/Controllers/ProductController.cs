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
            if (page <= 0) return BadRequest("Page value must be greater than 0");

            var products = await _productService.GetProductsAsync(token);
            if (!products.Value.Any()) return NoContent();

            var pageItemsResult = _productService.CalculatePageItems(products.Value, page);
            if (pageItemsResult.IsFailure) return NotFound();

            return View(pageItemsResult.Value);
        }

        [HttpGet]
        public async Task<IActionResult> ProductsByCategorySelector(CancellationToken token)
        {
            var categoriesResult = await _productService.GetCategoriesAsync(token);
            if (categoriesResult.IsFailure) return NoContent();

            return View(categoriesResult.Value);
        }

        [HttpGet("/Product/ProductsByCategory")]
        public async Task<IActionResult> ProductsByCategory(string category, CancellationToken token, int page = 1)
        {
            if (string.IsNullOrEmpty(category)) return BadRequest("Category is required.");
            if (page <= 0) return BadRequest("Page value must be greater than 0");

            var productsByCategoryResult = await _productService.GetProductsByCategoryAsync(category, token);
            if (productsByCategoryResult.IsFailure) return NoContent();

            var pageItemsResult = _productService.CalculatePageItems(productsByCategoryResult.Value, page);
            if (pageItemsResult.IsFailure) return NotFound();

            return PartialView("_ProductsListPartial", pageItemsResult.Value); 
        }
    }
}
