using Dislana.Application.Product.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dislana.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService service) => _productService = service;

        // GET api/products/filters?type=...
        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters([FromQuery] string type, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(type))
                return BadRequest(new { message = "El parámetro 'tipo' es requerido." });

            var items = await _productService.GetFiltersAsync(type, cancellationToken);
            return Ok(items);
        }

        // GET api/products/list?tipo=...
        [HttpGet("list")]
        public async Task<IActionResult> GetList([FromQuery] string type, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(type))
                return BadRequest(new { message = "El parámetro 'tipo' es requerido." });

            var items = await _productService.GetProductsAsync(type, cancellationToken);
            return Ok(items);
        }

        // GET api/products/detail?itemCode=...
        [HttpGet("detail")]
        public async Task<IActionResult> GetDetail([FromQuery] string itemCode, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(itemCode))
                return BadRequest(new { message = "El parámetro 'itemCode' es requerido." });

            var item = await _productService.GetProductDetailAsync(itemCode, cancellationToken);
            return Ok(item);
        }
    }
}
