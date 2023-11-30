namespace events_api.DTOs
{
    public class EventListResponseDTO
    {
        public int PagesAmount { get; set; }
        public List<EventDTO> Events { get; set; }
    }
}
