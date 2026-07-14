using AutoMapper;
using Moq;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Application.Mapping;
using ProductApi.Application.Services;
using ProductApi.Domain.Entities;
using ProductApi.Domain.Exceptions;

namespace ProductApi.Application.Tests;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<IRepository<Item>> _itemRepoMock;
    private readonly IMapper _mapper;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepoMock = new Mock<IProductRepository>();
        _itemRepoMock = new Mock<IRepository<Item>>();

        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Items).Returns(_itemRepoMock.Object);

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _sut = new ProductService(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task GetProductsAsync_ShouldReturnPagedResult()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), ProductName = "Test Product 1", CreatedBy = "user1", CreatedOn = DateTimeOffset.UtcNow, ModifiedBy = "user1", ModifiedOn = DateTimeOffset.UtcNow },
            new() { Id = Guid.NewGuid(), ProductName = "Test Product 2", CreatedBy = "user1", CreatedOn = DateTimeOffset.UtcNow, ModifiedBy = "user1", ModifiedOn = DateTimeOffset.UtcNow }
        };

        _productRepoMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(2);
        _productRepoMock.Setup(r => r.GetPagedWithItemsAsync(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(products);

        // Act
        var result = await _sut.GetProductsAsync(1, 10);

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithExistingId_ShouldReturnProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            ProductName = "Test Product",
            CreatedBy = "user1",
            CreatedOn = DateTimeOffset.UtcNow,
            ModifiedBy = "user1",
            ModifiedOn = DateTimeOffset.UtcNow
        };

        _productRepoMock.Setup(r => r.GetWithItemsAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        // Act
        var result = await _sut.GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Test Product", result.ProductName);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        _productRepoMock.Setup(r => r.GetWithItemsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.GetProductByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldCreateAndReturnProduct()
    {
        // Arrange
        var createDto = new CreateProductDto { ProductName = "New Product", CreatedBy = "user1" };

        _productRepoMock.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product p, CancellationToken ct) => p);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.CreateProductAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Product", result.ProductName);
        Assert.Equal("user1", result.CreatedBy);
        Assert.NotEqual(Guid.Empty, result.Id);

        _productRepoMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_WithExistingId_ShouldUpdateAndReturnProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var existingProduct = new Product
        {
            Id = productId,
            ProductName = "Old Name",
            CreatedBy = "user1",
            CreatedOn = DateTimeOffset.UtcNow,
            ModifiedBy = "user1",
            ModifiedOn = DateTimeOffset.UtcNow
        };

        var updateDto = new UpdateProductDto { ProductName = "Updated Name", ModifiedBy = "user2" };

        _productRepoMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(existingProduct);

        // Act
        var result = await _sut.UpdateProductAsync(productId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.ProductName);

        _productRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_WithNonExistingId_ShouldThrowNotFoundException()
    {
        // Arrange
        _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.UpdateProductAsync(Guid.NewGuid(), new UpdateProductDto { ProductName = "Name", ModifiedBy = "user" }));
    }

    [Fact]
    public async Task DeleteProductAsync_WithExistingId_ShouldDelete()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, ProductName = "Test", CreatedBy = "user", CreatedOn = DateTimeOffset.UtcNow, ModifiedBy = "user", ModifiedOn = DateTimeOffset.UtcNow };

        _productRepoMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        // Act
        await _sut.DeleteProductAsync(productId);

        // Assert
        _productRepoMock.Verify(r => r.DeleteAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_WithNonExistingId_ShouldThrowNotFoundException()
    {
        // Arrange
        _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.DeleteProductAsync(Guid.NewGuid()));
    }
}
