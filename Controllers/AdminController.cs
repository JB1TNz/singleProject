using Microsoft.AspNetCore.Mvc;
using singleProject.Models.Db;
using System.Linq;

namespace singleProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly EbookBestContext _db;

        public AdminController(EbookBestContext db)
        {
            _db = db;
        }

        public IActionResult Dashboard()
        {
            // Check if user is actually an admin (Optional but recommended)
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.TotalUsers = _db.UserData.Count();
            ViewBag.TotalProducts = _db.Products.Count();
            ViewBag.TotalSales = _db.UserLibraries.Count();
            
            // Calculate Total Revenue
            var revenue = (from lib in _db.UserLibraries
                          join p in _db.Products on lib.ProductId equals p.ProductId
                          select p.Price).Sum() ?? 0;
            
            ViewBag.TotalRevenue = revenue;

            // Optional: Recent Sales
            var recentSales = (from lib in _db.UserLibraries
                               join p in _db.Products on lib.ProductId equals p.ProductId
                               orderby lib.PurchaseDate descending
                               select new {
                                   ProductName = p.ProductName,
                                   PurchaseDate = lib.PurchaseDate,
                                   Price = p.Price
                               }).Take(5).ToList();
                               
            ViewBag.RecentSales = recentSales;

            return View();
        }

        public IActionResult SupportTickets()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Supporter")
            {
                return RedirectToAction("Index", "Home");
            }

            var tickets = (from t in _db.SupportTickets
                           join u in _db.UserData on t.UserId equals u.UserId into tu
                           from u in tu.DefaultIfEmpty()
                           orderby t.CreatedDate descending
                           select new singleProject.ViewModels.SupportTicketViewModel
                           {
                               Ticket = t,
                               UserEmail = u != null ? u.Email : "แฟ้มข้อมูลผู้เยี่ยมชม (Guest)"
                           }).ToList();

            return View(tickets);
        }

        [HttpPost]
        public IActionResult UpdateTicketStatus(int ticketId, string newStatus)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Supporter") return Unauthorized();

            var ticket = _db.SupportTickets.FirstOrDefault(t => t.TicketId == ticketId);
            if (ticket != null)
            {
                ticket.Status = newStatus;
                _db.SaveChanges();
            }
            return RedirectToAction("SupportTickets");
        }
    }
}
