namespace events_api.DTOs
{
    public class EventsFilter
    {
        public bool All { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public bool PerDate { get; set; }
        public bool PerTitle { get; set; }
        public bool PerUsers { get; set; }
        public bool Broadcasted { get; set; }
        public bool NotBroadcasted { get; set; }
        public int Page { get; set; } = 0;
        public int PerPage { get; set; } = 10;
    }
}
