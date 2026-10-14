using System;

namespace UniqueStudio.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public int BookingId { get; set; }
        public DateTime EventDate { get; set; }
        public string? Location { get; set; }
        public string? Photographer { get; set; }
    }
}