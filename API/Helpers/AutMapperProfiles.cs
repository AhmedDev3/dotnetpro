using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers;

public class AutMapperProfiles : Profile
{
    //ctor
    public AutMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>()
        .ForMember(d => d.Age, o => o.MapFrom(s => s.DateOfBirth.CalculateAge()))
        .ForMember(d => d.PhotoUrl, o =>
         o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain)!.Url));
        CreateMap<Photo, PhotoDto>();
        CreateMap<MemberUpdateDto, AppUser>();
        CreateMap<RegisterDto , AppUser>();
        // change from string to dateonly and fix the error
        CreateMap<string , DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
        CreateMap<Message ,MessageDto>()
            .ForMember(d => d.SenderPhotoUrl,
            o => o.MapFrom(s=> s.Sender.Photos.FirstOrDefault(x => x.IsMain)!.Url))
            .ForMember(d => d.RecipientPhotoUrl,
            o => o.MapFrom(s=> s.Recipient.Photos.FirstOrDefault(x => x.IsMain)!.Url));
        //fix the time read correct
        CreateMap<DateTime, DateTime> ().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?,DateTime?>().ConstructUsing(d => d.HasValue
        ? DateTime.SpecifyKind(d.Value , DateTimeKind.Utc) : null);
    }
}
