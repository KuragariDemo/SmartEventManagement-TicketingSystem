using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SmartEventManagement_TicketingSystem.Areas.Identity.Data
{
    public class SmartEventManagement_TicketingSystemUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Bio { get; set; }
        public string? EventPreferences { get; set; }
        public bool IsMember { get; set; } = false;


    }
}
