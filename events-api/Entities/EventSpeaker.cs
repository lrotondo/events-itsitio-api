namespace events_api.Entities
{
    public class EventSpeaker: AuditableEntity
    {
        public Guid EventId { get; set; }
        public Event Event { get; set; }
        public Guid SpeakerId { get; set; }
        public Speaker Speaker { get; set; }
    }
}
