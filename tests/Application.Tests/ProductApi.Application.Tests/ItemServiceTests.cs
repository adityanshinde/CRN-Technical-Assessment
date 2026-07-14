using AutoMapper;
using Moq;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Application.Mapping;
using ProductApi.Application.Services;
using ProductApi.Domain.Entities;
using ProductApi.Domain.Exceptions;

namespace ProductApi.Application.Tests;

public class ItemServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<IRepository<Item>> _itemRepoMock;
    private readonly IMapper _mapper;
    private readonly ItemService _sut;

    public ItemServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepoMock = new Mock<IProductRepository>();
        _itemRepoMock = new Mock<IRepository<Item>>();

        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Items).Returns(_itemRepoMock.Object);

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _sut = new ItemService(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task GetItemByIdAsync_WithExistingId_ShouldReturnItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = new Item { Id = itemId, ProductId = Guid.NewGuid(), Quantity = 5 };

        _itemRepoMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>())).ReturnsAsync(item);

        // Act
        var result = await _sut.GetItemByIdAsync(itemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(itemId, result.Id);
        Assert.Equal(5, result.Quantity);
    }

    [Fact]
    public async Task GetItemByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        _itemRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Item?)null);

        // Act
        var result = await _sut.GetItemByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateItemAsync_WithValidProductId_ShouldCreateItem()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var createDto = new CreateItemDto { ProductId = productId, Quantity = 10 };

        _productRepoMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _itemRepoMock.Setup(r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>())).ReturnsAsync((Item i, CancellationToken ct) => i);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.CreateItemAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.ProductId);
        Assert.Equal(10, result.Quantity);
        Assert.NotEqual(Guid.Empty, result.Id);

        _itemRepoMock.Verify(r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateItemAsync_WithNonExistingProductId_ShouldThrowNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var createDto = new CreateItemDto { ProductId = productId, Quantity = 10 };

        _productRepoMock.Setup(r => r.ExistsAsync(p => p.Id == productId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.CreateItemAsync(createDto));
    }

    [Fact]
    public async Task UpdateItemAsync_WithExistingId_ShouldUpdateItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = new Item { Id = itemId, ProductId = Guid.NewGuid(), Quantity = 5 };
        var updateDto = new UpdateItemDto { Quantity = 15 };

        _itemRepoMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>())).ReturnsAsync(existingItem);

        // Act
        var result = await _sut.UpdateItemAsync(itemId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(15, result.Quantity);

        _itemRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateItemAsync_WithNonExistingId_ShouldThrowNotFoundException()
    {
        // Arrange
        _itemRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Item?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.UpdateItemAsync(Guid.NewGuid(), new UpdateItemDto { Quantity = 10 }));
    }

    [Fact]
    public async Task DeleteItemAsync_WithExistingId_ShouldDelete()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = new Item { Id = itemId, ProductId = Guid.NewGuid(), Quantity = 5 };

        _itemRepoMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>())).ReturnsAsync(item);

        // Act
        await _sut.DeleteItemAsync(itemId);

        // Assert
        _itemRepoMock.Verify(r => r.DeleteAsync(item, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteItemAsync_WithNonExistingId_ShouldThrowNotFoundException()
    {
        // Arrange
        _itemRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Item?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteItemAsync(Guid.NewGuid()));
    }
}
