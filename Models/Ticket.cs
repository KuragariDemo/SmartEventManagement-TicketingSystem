using SmartEventManagement_TicketingSystem.Models.Events;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Ticket
{
    public int Id { get; set; }

    public int EventId { get; set; }
    public Event Event { get; set; }

    public string UserId { get; set; }

    public int Quantity { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime PurchaseDate { get; set; } = DateTime.Now;
}
