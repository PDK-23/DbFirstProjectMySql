using AutoMapper;
using DbFirstProjectMySql.Application.DTOs;
using DbFirstProjectMySql.Infrastructure.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, UserCreateDto>().ReverseMap();
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<Role, RoleDto>().ReverseMap();
        CreateMap<UserRegisterDto, UserCreateDto>();
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<ProductCreateDto, Product>();
        CreateMap<Role, RoleDto>().ReverseMap();
    }
}
