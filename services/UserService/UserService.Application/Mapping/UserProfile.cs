using AutoMapper;
using UserService.Domain.Entities;
using UserService.Application.DTOs;

namespace UserService.Application.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>();
        CreateMap<RegisterDto, User>();
        // Adicione outros mapeamentos conforme necessário
    }
} 