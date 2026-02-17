using System.ComponentModel.DataAnnotations;

namespace SmartEventManagement_TicketingSystem.Models
{
    public class Venue
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }
    }
}
