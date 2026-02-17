using System.Collections.Generic;
using SmartEventManagement_TicketingSystem.Models.Events;

public class AdminDashboardViewModel
{
    public List<string> Categories { get; set; }

    public List<string> Locations { get; set; }

    public List<Event> UpcomingEvents { get; set; } = new List<Event>();

    public List<Event> FinishedEvents { get; set; }
}
