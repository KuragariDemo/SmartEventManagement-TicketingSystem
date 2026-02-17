using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEventManagement_TicketingSystem.Models.Events
{
    public class EventSeat
    {
        public int Id { get; set; }  // MUST be auto identity

        public int EventId { get; set; }
        public Event Event { get; set; }

        public string SeatType { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }

}
