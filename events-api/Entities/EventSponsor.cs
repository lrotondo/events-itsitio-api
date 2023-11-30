namespace events_api.Entities
{
    public class EventSponsor: AuditableEntity
    {
        public Guid EventId { get; set; }
        public Event Event { get; set; }
        public Guid SponsorId { get; set; }
        public Sponsor Sponsor { get; set; }
    }
}
