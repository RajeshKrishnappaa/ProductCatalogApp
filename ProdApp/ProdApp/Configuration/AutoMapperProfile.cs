using AutoMapper;
using ProdApp.DTOS;
using ProdApp.Models;

namespace ProdApp.Configuration
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
       
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<ProductDTO, Product>()
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<ProductImageDTO, Product>()
                .ForMember(d => d.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
        }
    }
}
