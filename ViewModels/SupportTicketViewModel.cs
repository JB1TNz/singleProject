using System;
using singleProject.Models.Db;

namespace singleProject.ViewModels
{
    public class SupportTicketViewModel
    {
        public SupportTicket Ticket { get; set; } = null!;
        public string? UserEmail { get; set; }
    }
}
