using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace SmartEventManagement_TicketingSystem.Models.Events
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan EventTime { get; set; }

        public decimal Price { get; set; }   // ✅ FIXED

        public int CategoryId { get; set; }  // FK
        public int VenueId { get; set; }     // FK

        public string? ImageUrl { get; set; }

        // Navigation properties
        public Category? Category { get; set; }   // ✅ FIXED
        public Venue? Venue { get; set; }         // ✅ FIXED

        [NotMapped]
        public IFormFile? ImageFile { get; set; } // ✅ FIXED

        public ICollection<EventSeat>? Seats { get; set; }

    }
}
