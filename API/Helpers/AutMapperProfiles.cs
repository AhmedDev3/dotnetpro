using API.Data;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers;

public class AutMapperProfiles : Profile
{
    //ctor
    public AutMapperProfiles()
    {
        CreateMap<AppUser,MemberDto>()
        .ForMember(d => d.Age , o => o.MapFrom(s => s.DateOfBirth.CalculateAge()))
        .ForMember(d => d.PhotoUrl , o =>
         o.MapFrom(s => s.photos.FirstOrDefault(x => x.IsMain)!.Url));
        CreateMap<Photo , PhotoDto>();
    }
}
