using System.ComponentModel;

namespace events_api.Entities
{
    public class Event : AuditableEntity
    {
        public string Title { get; set; }
        public string PrimaryColor { get; set; }
        public string Description { get; set; }
        public string Subtitle { get; set; }
        public string Slug { get; set; }
        public DateTime DateUTC { get; set; }
        public List<EventSpeaker> Speakers { get; set; }
        public List<EventSponsor> Sponsors { get; set; }
        public string Banner { get; set; }
        public List<UserForEvent> Users { get; set; } = new List<UserForEvent>();
        public string ArenaId { get; set; }
        public string StreamId { get; set; }
        [DefaultValue(true)]
        public bool Live { get; set; } = true;
    }
}
