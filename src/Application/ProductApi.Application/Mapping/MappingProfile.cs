using AutoMapper;
using ProductApi.Application.DTOs;
using ProductApi.Domain.Entities;

namespace ProductApi.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        // Item mappings
        CreateMap<Item, ItemDto>();
        CreateMap<CreateItemDto, Item>();
        CreateMap<UpdateItemDto, Item>();
    }
}
