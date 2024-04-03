using events_api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace events_api.Services.Interfaces
{
    public interface IEventsServices
    {
        Task<ActionResult> CreateEvent(EventCreationDTO dto);
        Task<ActionResult<EventListResponseDTO>> GetAllEvents(EventsFilter filter);
        Task<ActionResult<EventDTO>> GetEvent(Guid id);
        Task<ActionResult<EventDTO>> GetEventBySlug(string slug);
        Task<ActionResult> AddSpeakerToEvent(Guid eventId, EventAddSpeakerDTO dto);
        Task<ActionResult> AddSponsorToEvent(Guid eventId, EventAddSponsorDTO dto);

        Task<ActionResult> AddModeratorToEvent(Guid eventId, EventAddModeratorDTO dto);
        Task<ActionResult> DeleteEvent(Guid eventId);
        Task<ActionResult> PutEvent(Guid id, EventPutDTO dto);
        Task<ActionResult> RemoveSponsorFromEvent(Guid id);
        Task<ActionResult> RemoveSpeakerFromEvent(Guid id);
        Task<ActionResult> RemoveModeratorFromEvent(Guid id);
        Task<ActionResult> AddUserToEvent(Guid eventId, RegisterUserToEventDTO dto);
        Task<ActionResult> TurnOffEvent(Guid eventId);
        Task<ActionResult<string>> GetUserFromEventReport(Guid eventId);
    }
}
