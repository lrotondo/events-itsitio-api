namespace events_api.Entities
{
    public class EventModerator : AuditableEntity
    {
        public Guid EventId { get; set; }
        public Event Event { get; set; }
        public Guid ModeratorId { get; set; }
        public Moderator Moderator { get; set; }
    }
}
