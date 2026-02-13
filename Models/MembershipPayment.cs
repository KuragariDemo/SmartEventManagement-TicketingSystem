using SmartEventManagement_TicketingSystem.Areas.Identity.Data;

public class MembershipPayment
{
    public int Id { get; set; }

    public string UserId { get; set; }
    public SmartEventManagement_TicketingSystemUser User { get; set; }

    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Status { get; set; }
}
