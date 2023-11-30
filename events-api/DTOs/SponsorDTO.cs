using events_api.Entities;

namespace events_api.DTOs
{
    public class SponsorDTO: AuditableEntity
    {
        public string Name { get; set; }
        public string Logo { get; set; }
    }
}
