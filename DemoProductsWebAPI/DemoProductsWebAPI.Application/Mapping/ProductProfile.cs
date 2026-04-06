using AutoMapper;
using DemoProductsWebAPI.Common.DTOs;
using DemoProductsWebAPI.Domain.Entities;

namespace DemoProductsWebAPI.Application.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, DemoProductsWebAPI.Common.DTOs.ProductDto>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.ProductName))
                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                .ForMember(d => d.ModifiedBy, o => o.MapFrom(s => s.ModifiedBy))
                .ForMember(d => d.ModifiedOn, o => o.MapFrom(s => s.ModifiedOn))
                .ReverseMap();
        }
    }
}
