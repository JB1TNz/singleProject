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
            var revenueQuery = (from lib in _db.UserLibraries
                                join p in _db.Products on lib.ProductId equals p.ProductId
                                select new {
                                    p.Price,
                                    p.PromotionPrice,
                                    p.PromotionEndDate,
                                    p.LastPromotionEdit,
                                    lib.PurchaseDate
                                }).ToList();

            decimal revenue = 0;
            foreach (var item in revenueQuery)
            {
                if (item.PromotionPrice != null && item.LastPromotionEdit != null && item.PromotionEndDate != null &&
                    item.PurchaseDate >= item.LastPromotionEdit && item.PurchaseDate <= item.PromotionEndDate)
                {
                    revenue += item.PromotionPrice.Value;
                }
                else
                {
                    revenue += item.Price ?? 0;
                }
            }
            
            ViewBag.TotalRevenue = revenue;

            // Optional: Recent Sales
            var recentSalesData = (from lib in _db.UserLibraries
                                   join p in _db.Products on lib.ProductId equals p.ProductId
                                   orderby lib.PurchaseDate descending
                                   select new {
                                       ProductName = p.ProductName,
                                       PurchaseDate = lib.PurchaseDate,
                                       Price = p.Price,
                                       PromotionPrice = p.PromotionPrice,
                                       PromotionEndDate = p.PromotionEndDate,
                                       LastPromotionEdit = p.LastPromotionEdit
                                   }).Take(5).ToList();

            var recentSales = recentSalesData.Select(x => new {
                ProductName = x.ProductName,
                PurchaseDate = x.PurchaseDate,
                Price = (x.PromotionPrice != null && x.LastPromotionEdit != null && x.PromotionEndDate != null &&
                         x.PurchaseDate >= x.LastPromotionEdit && x.PurchaseDate <= x.PromotionEndDate) 
                         ? x.PromotionPrice.Value : (x.Price ?? 0)
            }).ToList();
                               
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
