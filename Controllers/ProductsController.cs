using ECommerceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] bool activeOnly = false)
    {
        var products = activeOnly
            ? await _productService.GetActiveProductsAsync()
            : await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
        {
            return NotFound(new { message = "Product not found."});
        }

        return Ok(product);
    }

    // GET: api/products/slug/laptop-gaming
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ProductDto>> GetProductBySlug(string slug)
    {
        var product = await _productService.GetProductBySlugAsync(slug);

        if(product == null) return NotFound(new { message = "Product not found."});

        return Ok(product);
    }

    // POST: api/products
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        if (string.IsNullOrWhiteSpace(createProductDto.Name))
        {
            return BadRequest(new { message = "Product name is required" });
        }

        if (createProductDto.Price <= 0)
        {
            return BadRequest(new { message = "Price must be greater than 0" });
        }

        var product = await _productService.CreateProductAsync(createProductDto);
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id},  product);
    }

    // PUT: api/products/{id}
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updateProductDto)
    {
        var success = await _productService.UpdateProductAsync(id, updateProductDto);

        if (!success)
        {
            return NotFound(new { message = "Product not found"});
        }

        return NoContent();
    }

    // DELETE: api/products/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var success = await _productService.DeleteProductAsync(id);

        if(!success)
        {
            return NotFound(new { message = "Product not found"});
        }

        return NoContent();
    }

    // PATCH: api/products/{id}/stock
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/stock")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] int quantity)
    {
        var success = await _productService.UpdateStockAsync(id, quantity);

        if(!success)
        {
            return NotFound(new { message = "Product not found"});
        }

        return NoContent();
    }
}