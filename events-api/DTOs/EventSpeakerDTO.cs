using events_api.Entities;

namespace events_api.DTOs
{
    public class EventSpeakerDTO: AuditableEntity
    {
        public EventDTO Event { get; set; }
        public SpeakerDTO Speaker { get; set; }
    }
}
