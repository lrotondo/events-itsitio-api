namespace events_api.Entities
{
    public class Sponsor: AuditableEntity
    {
        public string Name { get; set; }
        public string Logo { get; set; }
    }
}
