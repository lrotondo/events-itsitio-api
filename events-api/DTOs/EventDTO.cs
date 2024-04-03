using events_api.Entities;

namespace events_api.DTOs
{
    public class EventDTO : AuditableEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PrimaryColor { get; set; }
        public string Subtitle { get; set; }
        public string Banner { get; set; }
        public DateTime DateUTC { get; set; }
        public List<EventSpeakerDTO> Speakers { get; set; }
        public List<EventSponsorDTO> Sponsors { get; set; }
        public List<EventModeratorDTO> Moderators { get; set; }
        public string Slug { get; set; }
        public string ArenaId { get; set; }
        public string StreamId { get; set; }
        public bool IsImage { get; set; }
        public List<UserForEventDTO> Users { get; set; }
        public bool Live { get; set; }
        public bool IsPast { get; set; }
    }
}
