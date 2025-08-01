using Microsoft.AspNetCore.Mvc;
using ShopOnline.DTOs;
using ShopOnline.Interfaces;
using ShopOnline.Mapper;
using ShopOnline.Models;
using ShopOnline.Services;

namespace ShopOnline.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("List")]
        public async Task<ActionResult> ProductList(int page = 1, int? category = null)
        {
            var products = await _productService.ProductListInPages(page, category);
            return Ok(products);
        }

        [HttpGet("Detail/{id}")]
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var product = await _productService.ProductDetails(id);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = new ProductMapper().MapProduct(productDto);
            var created = await _productService.AddProduct(product);
            return CreatedAtAction(nameof(Details), new { id = created?.ProductId }, created);
        }
    }
}