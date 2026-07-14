using AutoMapper;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Domain.Exceptions;

namespace ProductApi.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await _unitOfWork.Products.CountAsync(cancellationToken);
        var products = await _unitOfWork.Products.GetPagedWithItemsAsync(pageNumber, pageSize, cancellationToken);

        return new PagedResult<ProductDto>
        {
            Items = _mapper.Map<IReadOnlyList<ProductDto>>(products),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetWithItemsAsync(id, cancellationToken);
        return product is null ? null : _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto, CancellationToken cancellationToken = default)
    {
        var product = _mapper.Map<Product>(createDto);
        product.Id = Guid.NewGuid();
        product.CreatedOn = DateTimeOffset.UtcNow;
        product.ModifiedOn = DateTimeOffset.UtcNow;
        product.ModifiedBy = createDto.CreatedBy;

        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto updateDto, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (product is null)
            throw new NotFoundException(nameof(Product), id);

        _mapper.Map(updateDto, product);
        product.ModifiedBy = updateDto.ModifiedBy;
        product.ModifiedOn = DateTimeOffset.UtcNow;

        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductDto>(product);
    }

    public async Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (product is null)
            throw new NotFoundException(nameof(Product), id);

        await _unitOfWork.Products.DeleteAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<ItemDto>> GetProductItemsAsync(Guid productId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var productExists = await _unitOfWork.Products.ExistsAsync(p => p.Id == productId, cancellationToken);
        if (!productExists)
            throw new NotFoundException(nameof(Product), productId);

        var items = await _unitOfWork.Items.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        var filteredItems = items.Where(i => i.ProductId == productId).ToList();
        var totalCount = filteredItems.Count;

        return new PagedResult<ItemDto>
        {
            Items = _mapper.Map<IReadOnlyList<ItemDto>>(filteredItems),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
