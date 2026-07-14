using ProductApi.Application.DTOs;

namespace ProductApi.Application.Interfaces;

public interface IItemService
{
    Task<ItemDto?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ItemDto> CreateItemAsync(CreateItemDto createDto, CancellationToken cancellationToken = default);
    Task<ItemDto> UpdateItemAsync(Guid id, UpdateItemDto updateDto, CancellationToken cancellationToken = default);
    Task DeleteItemAsync(Guid id, CancellationToken cancellationToken = default);
}
