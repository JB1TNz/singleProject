using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using singleProject.Models;
using singleProject.Models.Db;


namespace singleProject.Controllers;

public class HomeController : Controller
{
    private readonly EbookBestContext _db;
    public HomeController(EbookBestContext db)
    {
        _db = db;
    }

    public IActionResult Lab08()
    {   
        var user = (from u in _db.UserData select u ).ToList();
        return View(user);
    }

    public IActionResult Index(int? categoryId)
    {
        IQueryable<ProductData> query = _db.Products.Where(p => p.Status == 1);
        
        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        var products = query.OrderByDescending(p => p.CreatedDate).ToList();
        
        // Fetch promoted books
        var promotedBooks = _db.Products
            .Where(p => p.Status == 1 && p.PromotionPrice != null && p.PromotionEndDate >= DateTime.Now)
            .OrderByDescending(p => p.LastPromotionEdit)
            .Take(4)
            .ToList();
            
        ViewBag.SelectedCategory = categoryId;
        ViewBag.PromotedBooks = promotedBooks;
        
        return View(products);
    }

    public IActionResult BookPreview(int id)
    {
        var product = _db.Products.FirstOrDefault(p => p.ProductId == id);
        if (product == null)
            return NotFound();
            
        // Check if user already owns it
        var userId = HttpContext.Session.GetString("UserId");
        if (!string.IsNullOrEmpty(userId))
        {
            ViewBag.AlreadyOwned = _db.UserLibraries.Any(l => l.UserId == userId && l.ProductId == id);
        }
        else
        {
            ViewBag.AlreadyOwned = false;
        }
            
        return View(product);
    }

    [HttpPost]
    public IActionResult BuyBook(int id)
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        var product = _db.Products.FirstOrDefault(p => p.ProductId == id);
        if (product == null) return NotFound();

        // Check if already bought
        var alreadyOwned = _db.UserLibraries.Any(l => l.UserId == userId && l.ProductId == id);
        if (!alreadyOwned)
        {
            _db.UserLibraries.Add(new UserLibrary 
            { 
                UserId = userId, 
                ProductId = id, 
                PurchaseDate = DateTime.Now 
            });
            _db.SaveChanges();
            TempData["SuccessMessage"] = "สั่งซื้อหนังสือเรียบร้อยแล้ว!";
        }

        return RedirectToAction("MyLibrary");
    }

    public IActionResult MyLibrary()
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        // Join UserLibrary and Products
        var library = (from lib in _db.UserLibraries
                       join p in _db.Products on lib.ProductId equals p.ProductId
                       where lib.UserId == userId
                       orderby lib.PurchaseDate descending
                       select new 
                       {
                           Product = p,
                           PurchaseDate = lib.PurchaseDate
                       }).ToList();

        // Pass to view (using Dynamic or ViewModel, here we can just use dynamic or pass Products)
        // Since View takes strongly typed mode, we'll pass the list of products directly and store dates in ViewBag if needed.
        // For simplicity, passing List<ProductData>
        var products = library.Select(x => x.Product).ToList();
        return View(products);
    }

    public IActionResult Supporter()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Supporter(string topic, string description)
    {
        if (!string.IsNullOrEmpty(topic) && !string.IsNullOrEmpty(description))
        {
            var userId = HttpContext.Session.GetString("UserId") ?? "Guest";
            _db.SupportTickets.Add(new SupportTicket
            {
                UserId = userId,
                Topic = topic,
                Description = description,
                Status = "Open",
                CreatedDate = DateTime.Now
            });
            _db.SaveChanges();
            TempData["SuccessMessage"] = "ส่งเรื่องแจ้งทีมงานเรียบร้อยแล้ว เราจะตรวจสอบโดยเร็วที่สุด!";
            return RedirectToAction("Supporter");
        }
        
        ModelState.AddModelError("", "กรุณากรอกข้อมูลให้ครบถ้วน");
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
