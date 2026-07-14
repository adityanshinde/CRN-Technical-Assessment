using AutoMapper;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Domain.Exceptions;

namespace ProductApi.Application.Services;

public class ItemService : IItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ItemService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ItemDto?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(id, cancellationToken);
        return item is null ? null : _mapper.Map<ItemDto>(item);
    }

    public async Task<ItemDto> CreateItemAsync(CreateItemDto createDto, CancellationToken cancellationToken = default)
    {
        var productExists = await _unitOfWork.Products.ExistsAsync(p => p.Id == createDto.ProductId, cancellationToken);
        if (!productExists)
            throw new NotFoundException(nameof(Product), createDto.ProductId);

        var item = _mapper.Map<Item>(createDto);
        item.Id = Guid.NewGuid();

        await _unitOfWork.Items.AddAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ItemDto>(item);
    }

    public async Task<ItemDto> UpdateItemAsync(Guid id, UpdateItemDto updateDto, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(id, cancellationToken);
        if (item is null)
            throw new NotFoundException(nameof(Item), id);

        _mapper.Map(updateDto, item);

        await _unitOfWork.Items.UpdateAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ItemDto>(item);
    }

    public async Task DeleteItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(id, cancellationToken);
        if (item is null)
            throw new NotFoundException(nameof(Item), id);

        await _unitOfWork.Items.DeleteAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
