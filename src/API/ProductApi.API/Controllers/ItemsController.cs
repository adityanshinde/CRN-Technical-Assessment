using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;

namespace ProductApi.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/items")]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemsController(IItemService itemService)
    {
        _itemService = itemService;
    }

    /// <summary>
    /// Gets an item by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ItemDto>> GetItem(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _itemService.GetItemByIdAsync(id, cancellationToken);
        if (item is null)
            return NotFound(new { error = "Item not found.", detail = $"Item with ID {id} was not found." });

        return Ok(item);
    }

    /// <summary>
    /// Creates a new item.
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ItemDto>> CreateItem(
        CreateItemDto createDto,
        CancellationToken cancellationToken = default)
    {
        var item = await _itemService.CreateItemAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
    }

    /// <summary>
    /// Updates an existing item.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ItemDto>> UpdateItem(
        Guid id,
        UpdateItemDto updateDto,
        CancellationToken cancellationToken = default)
    {
        var item = await _itemService.UpdateItemAsync(id, updateDto, cancellationToken);
        return Ok(item);
    }

    /// <summary>
    /// Deletes an item.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> DeleteItem(Guid id, CancellationToken cancellationToken = default)
    {
        await _itemService.DeleteItemAsync(id, cancellationToken);
        return NoContent();
    }
}
