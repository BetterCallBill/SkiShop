using API.Dtos;
using AutoMapper;
using Core.Entities;

namespace API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // destinationMember => ProductToReturnDto
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.ProductBrand, option => option.MapFrom(source => source.ProductBrand.Name))
                .ForMember(d => d.ProductType, option => option.MapFrom(source => source.ProductType.Name))
                .ForMember(d => d.PictureUrl, option => option.MapFrom<ProductUrlResolver>());
        }
    }
}