using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Data.Repositories;

namespace ProductApi.Infrastructure.Tests;

public class ProductRepositoryTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ProductRepository _sut;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"ProductApiTestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _sut = new ProductRepository(_dbContext);

        SeedData();
    }

    private void SeedData()
    {
        var products = new List<Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ProductName = "Product 1",
                CreatedBy = "test",
                CreatedOn = DateTimeOffset.UtcNow,
                ModifiedBy = "test",
                ModifiedOn = DateTimeOffset.UtcNow,
                Items = new List<Item>
                {
                    new() { Id = Guid.NewGuid(), ProductId = Guid.Empty, Quantity = 5 },
                    new() { Id = Guid.NewGuid(), ProductId = Guid.Empty, Quantity = 10 }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProductName = "Product 2",
                CreatedBy = "test",
                CreatedOn = DateTimeOffset.UtcNow,
                ModifiedBy = "test",
                ModifiedOn = DateTimeOffset.UtcNow
            }
        };

        // Fix foreign keys
        var itemsList = products[0].Items.ToList();
        itemsList[0].ProductId = products[0].Id;
        itemsList[1].ProductId = products[0].Id;

        _dbContext.Products.AddRange(products);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task GetWithItemsAsync_ShouldIncludeItems()
    {
        // Arrange
        var product = await _dbContext.Products.FirstAsync(p => p.Items.Any());

        // Act
        var result = await _sut.GetWithItemsAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetWithItemsAsync_ForProductWithoutItems_ShouldReturnEmptyItems()
    {
        // Arrange
        var product = await _dbContext.Products.FirstAsync(p => !p.Items.Any());

        // Act
        var result = await _sut.GetWithItemsAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetPagedWithItemsAsync_ShouldReturnProductsWithItems()
    {
        // Act
        var result = await _sut.GetPagedWithItemsAsync(1, 10);

        // Assert
        Assert.Equal(2, result.Count);
    }
}
