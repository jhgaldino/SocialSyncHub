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

        CreateMap<SocialAccount, SocialAccountDto>()
            .ForMember(dest => dest.NetworkType, 
                      opt => opt.MapFrom(src => (SocialNetworkTypeDto)src.NetworkType));
        
        CreateMap<ConnectSocialAccountDto, SocialAccount>()
            .ForMember(dest => dest.NetworkType, 
                      opt => opt.MapFrom(src => (SocialNetworkType)src.NetworkType));
    }
} 