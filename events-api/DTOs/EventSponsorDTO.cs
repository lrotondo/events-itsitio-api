using events_api.Entities;

namespace events_api.DTOs
{
    public class EventSponsorDTO: AuditableEntity
    {
        public EventDTO Event { get; set; }
        public SponsorDTO Sponsor { get; set; }
    }
}
