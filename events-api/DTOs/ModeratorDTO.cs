using events_api.Entities;

namespace events_api.DTOs
{
    public class ModeratorDTO : AuditableEntity
    {
        public string Name { get; set; }
        public string Picture { get; set; }
        public string Description { get; set; }
        public string Video { get; set; }
        public string Bio { get; set; }
    }
}
