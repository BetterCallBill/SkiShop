using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Entities.OrderAggregate;

namespace API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // source, destination
            // destinationMember => ProductToReturnDto
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.ProductBrand, option => option.MapFrom(source => source.ProductBrand.Name))
                .ForMember(d => d.ProductType, option => option.MapFrom(source => source.ProductType.Name))
                .ForMember(d => d.PictureUrl, option => option.MapFrom<ProductUrlResolver>());
                
            CreateMap<Core.Entities.Identity.Address, AddressDto>().ReverseMap();
            CreateMap<CustomerBasketDto, CustomerBasket>().ReverseMap();
            CreateMap<BasketItemDto, BasketItem>().ReverseMap();
            CreateMap<AddressDto, Core.Entities.OrderAggregate.Address>();
            
            CreateMap<Order, OrderToReturnDto>()
                // destinationMember, options
                // .ForMember(OrderToReturnDto => OrderToReturnDto.DeliveryMethod, option => option.MapFrom(Order => Order.DeliveryMethod.ShortName))
                .ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(d => d.ShippingPrice, o => o.MapFrom(s => s.DeliveryMethod.Price));
                
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ItemOrdered.ProductItemId))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.ItemOrdered.ProductName))
                .ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.ItemOrdered.PictureUrl))
                // Map destination member using a custom value resolver
                .ForMember(d => d.PictureUrl, o => o.MapFrom<OrderItemUrlResolver>());
        }
    }
}