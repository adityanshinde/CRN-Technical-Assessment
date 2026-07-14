using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Data.Repositories;

namespace ProductApi.Infrastructure.Tests;

public class RepositoryTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Repository<Product> _sut;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"ProductApiTestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _sut = new Repository<Product>(_dbContext);

        // Seed data
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
                ModifiedOn = DateTimeOffset.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProductName = "Product 2",
                CreatedBy = "test",
                CreatedOn = DateTimeOffset.UtcNow,
                ModifiedBy = "test",
                ModifiedOn = DateTimeOffset.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProductName = "Product 3",
                CreatedBy = "test",
                CreatedOn = DateTimeOffset.UtcNow,
                ModifiedBy = "test",
                ModifiedOn = DateTimeOffset.UtcNow
            }
        };

        _dbContext.Products.AddRange(products);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnEntity()
    {
        // Arrange
        var product = await _dbContext.Products.FirstAsync();

        // Act
        var result = await _sut.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.ProductName, result.ProductName);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectPage()
    {
        // Act
        var page1 = await _sut.GetPagedAsync(1, 2);
        var page2 = await _sut.GetPagedAsync(2, 2);

        // Assert
        Assert.Equal(2, page1.Count);
        Assert.Single(page2);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            ProductName = "New Product",
            CreatedBy = "test",
            CreatedOn = DateTimeOffset.UtcNow,
            ModifiedBy = "test",
            ModifiedOn = DateTimeOffset.UtcNow
        };

        // Act
        await _sut.AddAsync(product);
        await _dbContext.SaveChangesAsync();

        // Assert
        var result = await _dbContext.Products.FindAsync(product.Id);
        Assert.NotNull(result);
        Assert.Equal("New Product", result.ProductName);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        // Act
        var count = await _sut.CountAsync();

        // Assert
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrueForExistingEntity()
    {
        // Arrange
        var product = await _dbContext.Products.FirstAsync();

        // Act
        var exists = await _sut.ExistsAsync(p => p.Id == product.Id);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalseForNonExistingEntity()
    {
        // Act
        var exists = await _sut.ExistsAsync(p => p.Id == Guid.NewGuid());

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        var product = await _dbContext.Products.FirstAsync();

        // Act
        await _sut.DeleteAsync(product);
        await _dbContext.SaveChangesAsync();

        // Assert
        var result = await _dbContext.Products.FindAsync(product.Id);
        Assert.Null(result);
    }
}
