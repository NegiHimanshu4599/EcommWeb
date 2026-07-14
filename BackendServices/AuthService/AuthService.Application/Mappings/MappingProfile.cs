using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using AutoMapper;

namespace AuthService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Auth
            CreateMap<RegisterUserDto, ApplicationUser>()
                .ForMember(d => d.UserName,o => o.MapFrom(s => s.UserName))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
                .ForAllMembers(o => o.Condition((src, dest, value) => value != null));
            CreateMap<GoogleLoginDto, ApplicationUser>()
                .ForMember(d => d.UserName,o => o.MapFrom(s => s.Email))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
                .ForAllMembers(o => o.Condition((src, dest, value) => value != null));
            CreateMap<ApplicationUser, LoginResponseDto>()
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.UserProfile != null ? s.UserProfile.FullName : s.UserName))
                .ForMember(d => d.AccessToken, o => o.Ignore())
                .ForMember(d => d.RefreshToken,o => o.Ignore())
                .ForMember(d => d.Role,o => o.Ignore())
                .ForMember(d => d.AccessTokenExpiry,o => o.Ignore())
                .ForMember(d => d.IsProfileComplete, o => o.Ignore());
            #endregion

            #region Address
            CreateMap<CreateAddressDto, Address>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.UserId, o => o.Ignore())
                .ForMember(d => d.User, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore());
            CreateMap<UpdateAddressDto, Address>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.UserId, o => o.Ignore())
                .ForMember(d => d.User, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore());
            CreateMap<Address, AddressDto>();
            #endregion

            #region UserProfile
            CreateMap<UpdateUserProfileDto, UserProfile>()
                .ForMember(d => d.UserId, o => o.Ignore())
                .ForMember(d => d.User, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore());
            CreateMap<UserProfile, UserProfileDto>();
            #endregion
        }
    }
}