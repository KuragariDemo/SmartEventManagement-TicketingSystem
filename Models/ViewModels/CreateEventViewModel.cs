using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SmartEventManagement_TicketingSystem.Models.Events;

public class CreateEventViewModel
{
    public Event NewEvent { get; set; }

    public List<Event> Events { get; set; }

    public IFormFile? ImageFile { get; set; }
}
