using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class TicketPurchaseViewModel
{
    public int EventId { get; set; }

    public List<SeatSelection> Seats { get; set; } = new List<SeatSelection>();
}

public class SeatSelection
{
    public int SeatId { get; set; }

    [Range(0, 100)]
    public int Quantity { get; set; }
}
