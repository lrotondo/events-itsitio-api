namespace events_api.Entities
{
    public class UserForEvent : AuditableEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Company { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public Event Event { get; set; }
    }
}
