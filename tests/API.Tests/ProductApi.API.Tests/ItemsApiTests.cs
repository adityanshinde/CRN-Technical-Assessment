using System.Net;
using System.Net.Http.Json;
using ProductApi.Application.DTOs;

namespace ProductApi.API.Tests;

public class ItemsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ItemsApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetItem_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/v1/items/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateItem_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createDto = new CreateItemDto { ProductId = Guid.NewGuid(), Quantity = 5 };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/items", createDto);

        // Assert - POST is auth-required
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateItem_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var updateDto = new UpdateItemDto { Quantity = 10 };

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/items/{Guid.NewGuid()}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteItem_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/v1/items/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
