using System;
using System.ComponentModel.DataAnnotations;

namespace SmartEventManagement_TicketingSystem.Models.Events
{
    public class Event
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }

        public string Location { get; set; }

        public DateTime EventDate { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }
    }

}
