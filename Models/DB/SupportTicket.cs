using System;

namespace singleProject.Models.Db;

public partial class SupportTicket
{
    public int TicketId { get; set; }
    public string? UserId { get; set; }
    public string? Topic { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; } // Open, Closed
    public DateTime? CreatedDate { get; set; }
}
