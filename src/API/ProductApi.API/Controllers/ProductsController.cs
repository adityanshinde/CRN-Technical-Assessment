using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;

namespace ProductApi.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Gets a paginated list of products.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
        [FromQuery] PaginationParams pagination,
        CancellationToken cancellationToken = default)
    {
        var result = await _productService.GetProductsAsync(pagination.PageNumber, pagination.PageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a product by ID, including its items.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productService.GetProductByIdAsync(id, cancellationToken);
        if (product is null)
            return NotFound(new { error = "Product not found.", detail = $"Product with ID {id} was not found." });

        return Ok(product);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        CreateProductDto createDto,
        CancellationToken cancellationToken = default)
    {
        var product = await _productService.CreateProductAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ProductDto>> UpdateProduct(
        Guid id,
        UpdateProductDto updateDto,
        CancellationToken cancellationToken = default)
    {
        var product = await _productService.UpdateProductAsync(id, updateDto, cancellationToken);
        return Ok(product);
    }

    /// <summary>
    /// Deletes a product.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken = default)
    {
        await _productService.DeleteProductAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Gets paginated items for a specific product.
    /// </summary>
    [HttpGet("{id:guid}/items")]
    public async Task<ActionResult<PagedResult<ItemDto>>> GetProductItems(
        Guid id,
        [FromQuery] PaginationParams pagination,
        CancellationToken cancellationToken = default)
    {
        var result = await _productService.GetProductItemsAsync(id, pagination.PageNumber, pagination.PageSize, cancellationToken);
        return Ok(result);
    }
}
