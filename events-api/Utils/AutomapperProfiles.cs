using AutoMapper;
using events_api.DTOs;
using events_api.Entities;

namespace events_api.Utils
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<ApplicationUser, ApplicationUserPublic>();
            CreateMap<SignUpDTO, ApplicationUser>();

            CreateMap<EventCreationDTO, Event>()
                .ForMember(e => e.DateUTC, o => o.MapFrom(dto => DateTime.SpecifyKind(dto.DateUTC, DateTimeKind.Utc)));
            CreateMap<EventPutDTO, Event>()
                .ForMember(e => e.DateUTC, o => o.MapFrom(dto => DateTime.SpecifyKind(dto.DateUTC, DateTimeKind.Utc)));

            CreateMap<Event, EventDTO>()
                .ForMember(dto => dto.IsPast, o => o.MapFrom(e => e.DateUTC.ToUniversalTime() < DateTime.UtcNow));

            CreateMap<Speaker, SpeakerDTO>();
            CreateMap<Sponsor, SponsorDTO>();
            CreateMap<Moderator, ModeratorDTO>();

            CreateMap<EventSpeaker, EventSpeakerDTO>();
            CreateMap<EventSponsor, EventSponsorDTO>();
            CreateMap<EventModerator, EventModeratorDTO>();

            CreateMap<EventAddSpeakerDTO, Speaker>();
            CreateMap<EventAddSponsorDTO, Sponsor>();
            CreateMap<EventAddModeratorDTO, Moderator>();

            CreateMap<RegisterUserToEventDTO, UserForEvent>();
            CreateMap<UserForEvent, UserForEventDTO>();
        }
    }
}
