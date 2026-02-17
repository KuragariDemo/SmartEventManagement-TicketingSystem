using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartEventManagement_TicketingSystem.Areas.Identity.Data;
using SmartEventManagement_TicketingSystem.Models;
using SmartEventManagement_TicketingSystem.Models.Events;

namespace SmartEventManagement_TicketingSystem.Data;

public class SmartEventManagement_TicketingSystemContext : IdentityDbContext<SmartEventManagement_TicketingSystemUser>
{
    public SmartEventManagement_TicketingSystemContext(DbContextOptions<SmartEventManagement_TicketingSystemContext> options)
        : base(options)
    {
    }

    public DbSet<MembershipPayment> MembershipPayments { get; set; }

    public DbSet<Event> Events { get; set; }
    public DbSet<Venue> Venues { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<EventSeat> EventSeats { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
      
    }
}
