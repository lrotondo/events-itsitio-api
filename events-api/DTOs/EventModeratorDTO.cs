using events_api.Entities;

namespace events_api.DTOs
{
    public class EventModeratorDTO: AuditableEntity
    {
        public EventDTO Event { get; set; }
        public ModeratorDTO Moderator { get; set; }
    }
}
