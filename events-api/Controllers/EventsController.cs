using events_api.DTOs;
using events_api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace events_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EventsController : ControllerBase
    {
        private readonly IEventsServices eventsServices;

        public EventsController(IEventsServices eventsServices)
        {
            this.eventsServices = eventsServices;
        }

        [HttpGet]
        public async Task<ActionResult<EventListResponseDTO>> GetAllEvents([FromQuery] EventsFilter filter)
        {
            return await eventsServices.GetAllEvents(filter);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventDTO>> GetEvent(Guid id)
        {
            return await eventsServices.GetEvent(id);
        }

        [HttpGet("by-slug/{slug}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventDTO>> GetEventBySlug(string slug)
        {
            return await eventsServices.GetEventBySlug(slug);
        }

        [HttpPost]
        public async Task<ActionResult> CreateEvent(EventCreationDTO dto)
        {
            return await eventsServices.CreateEvent(dto);
        }

        [HttpPost("{id}/speakers")]
        public async Task<ActionResult> AddSpeakerToEvent([FromRoute] Guid id, [FromBody] EventAddSpeakerDTO dto)
        {
            return await eventsServices.AddSpeakerToEvent(id, dto);
        }

        [HttpPost("{id}/sponsors")]
        public async Task<ActionResult> AddSponsorToEvent([FromRoute] Guid id, [FromBody] EventAddSponsorDTO dto)
        {
            return await eventsServices.AddSponsorToEvent(id, dto);
        }

        [HttpPost("{id}/moderators")]
        public async Task<ActionResult> AddModeratorToEvent([FromRoute] Guid id, [FromBody] EventAddModeratorDTO dto)
        {
            return await eventsServices.AddModeratorToEvent(id, dto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEvent([FromRoute] Guid id)
        {
            return await eventsServices.DeleteEvent(id);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutEvent([FromRoute] Guid id, [FromBody] EventPutDTO dto)
        {
            return await eventsServices.PutEvent(id, dto);
        }

        [HttpDelete("speakers/{id}")]
        public async Task<ActionResult> RemoveSpeakerFromEvent([FromRoute] Guid id)
        {
            return await eventsServices.RemoveSpeakerFromEvent(id);
        }

        [HttpDelete("sponsors/{id}")]
        public async Task<ActionResult> RemoveSponsorFromEvent([FromRoute] Guid id)
        {
            return await eventsServices.RemoveSponsorFromEvent(id);
        }

        [HttpDelete("moderators/{id}")]
        public async Task<ActionResult> RemoveModeratorsFromEvent([FromRoute] Guid id)
        {
            return await eventsServices.RemoveModeratorFromEvent(id);
        }

        [HttpPost("{eventId}/register")]
        [AllowAnonymous]
        public async Task<ActionResult> AddUserToEvent([FromRoute] Guid eventId, [FromBody] RegisterUserToEventDTO dto)
        {
            return await eventsServices.AddUserToEvent(eventId, dto);
        }

        [HttpPost("{id}/off")]
        public async Task<ActionResult> TurnOffEvent([FromRoute] Guid id)
        {
            return await eventsServices.TurnOffEvent(id);
        }

        [HttpGet("{id}/report")]
        public async Task<ActionResult<string>> GetUserFromEventReport([FromRoute] Guid id)
        {
            return await eventsServices.GetUserFromEventReport(id);
        }
    }
}
