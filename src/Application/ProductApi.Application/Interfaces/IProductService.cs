using ProductApi.Application.DTOs;

namespace ProductApi.Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateProductAsync(CreateProductDto createDto, CancellationToken cancellationToken = default);
    Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto updateDto, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<ItemDto>> GetProductItemsAsync(Guid productId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
