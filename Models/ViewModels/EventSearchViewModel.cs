using SmartEventManagement_TicketingSystem.Models;
using SmartEventManagement_TicketingSystem.Models.Events;
using System;
using System.Collections.Generic;

public class EventSearchViewModel
{
    public List<Event> Events { get; set; }

    public string SearchCategory { get; set; }
    public string SearchLocation { get; set; }
    public DateTime? SearchDate { get; set; }
    public decimal? MaxPrice { get; set; }
}
