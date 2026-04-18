// SellerController.cs
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using singleProject.Models;
using singleProject.Models.Db;
using singleProject.ViewModels;
using Microsoft.AspNetCore.Http;

namespace singleProject.Controllers;

public class SellerController : Controller
{
    private readonly EbookBestContext _db;
    private readonly IWebHostEnvironment _env;

    public SellerController(EbookBestContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public IActionResult Dashboard()
    {
        var sellerId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(sellerId)) return RedirectToAction("Login", "Account");

        var products = _db.Products.Where(p => p.SellerId == sellerId).ToList();
        
        // Count downloads/purchases per product
        var productStats = products.Select(p => new 
        {
            Product = p,
            DownloadCount = _db.UserLibraries.Count(l => l.ProductId == p.ProductId)
        }).ToList();
        
        ViewBag.TotalProducts = products.Count;
        ViewBag.TotalDownloads = productStats.Sum(ps => ps.DownloadCount);

        // Map to dynamic list for view
        return View(productStats);
    }

    public IActionResult EditProduct()
    {
        var sellerId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(sellerId)) return RedirectToAction("Login", "Account");

        var products = _db.Products.Where(p => p.SellerId == sellerId).OrderByDescending(p => p.CreatedDate).ToList();
        return View(products);
    }

    // GET: /Seller/EditProductDetail/5
    public IActionResult EditProductDetail(int id)
    {
        var sellerId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(sellerId)) return RedirectToAction("Login", "Account");

        var product = _db.Products.FirstOrDefault(p => p.ProductId == id && p.SellerId == sellerId);
        if (product == null) return NotFound();

        var model = new EditProductViewModel
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName ?? string.Empty,
            ProductDescription = product.ProductDescription,
            Price = product.Price ?? 0,
            CategoryId = product.CategoryId ?? 0,
            Status = product.Status ?? 1,
            ExistingCoverPicture = product.CoverPicture,
            PromotionPrice = product.PromotionPrice,
            PromotionEndDate = product.PromotionEndDate,
            LastPromotionEdit = product.LastPromotionEdit
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProductDetail(EditProductViewModel model)
    {
        var sellerId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(sellerId)) return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        var product = _db.Products.FirstOrDefault(p => p.ProductId == model.ProductId && p.SellerId == sellerId);
        if (product == null) return NotFound();

        // Promotion limit: Can only edit promotion once every 24 hours
        if (model.PromotionPrice != product.PromotionPrice || model.PromotionEndDate != product.PromotionEndDate)
        {
            if (product.LastPromotionEdit.HasValue && (DateTime.Now - product.LastPromotionEdit.Value).TotalHours < 24)
            {
                ModelState.AddModelError("", "ไม่สามารถแก้ไขโปรโมชั่นได้บ่อยกว่า 1 ครั้งใน 24 ชั่วโมง หรือ 1 วัน");
                return View(model);
            }
            
            product.PromotionPrice = model.PromotionPrice;
            product.PromotionEndDate = model.PromotionEndDate;
            product.LastPromotionEdit = DateTime.Now;
        }

        // Update properties
        product.ProductName = model.ProductName;
        product.ProductDescription = model.ProductDescription;
        product.Price = model.Price;
        product.CategoryId = model.CategoryId;
        product.Status = model.Status;
        product.UpdatedDate = DateTime.Now;

        // If new cover image is uploaded
        if (model.CoverImage != null && model.CoverImage.Length > 0)
        {
            string coversFolder = Path.Combine(_env.WebRootPath, "uploads", "covers");
            Directory.CreateDirectory(coversFolder);

            string coverFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.CoverImage.FileName)}";
            string coverFilePath = Path.Combine(coversFolder, coverFileName);
            using (var stream = new FileStream(coverFilePath, FileMode.Create))
            {
                await model.CoverImage.CopyToAsync(stream);
            }
            product.CoverPicture = $"/uploads/covers/{coverFileName}";
        }

        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = $"แก้ไขข้อมูลสินค้า \"{product.ProductName}\" สำเร็จ!";
        return RedirectToAction("EditProduct");
    }

    // GET: /Seller/UploadProduct 
    public IActionResult UploadProduct()
    {
        // ถ้ายังไม่ login ให้ redirect ไป Login
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            return RedirectToAction("Login", "Account");

        return View(new ProductViewModel());
    }

    // POST: /Seller/UploadProduct
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadProduct(ProductViewModel model)
    {
        // ดึง SellerId จาก Session
        var sellerId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(sellerId))
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        // -------- บันทึกไฟล์สินค้า --------
        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "products");
        Directory.CreateDirectory(uploadsFolder);

        string productFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.File.FileName)}";
        string productFilePath = Path.Combine(uploadsFolder, productFileName);
        using (var stream = new FileStream(productFilePath, FileMode.Create))
        {
            await model.File.CopyToAsync(stream);
        }

        // -------- บันทึกภาพหน้าปก (ถ้ามี) --------
        string? coverPath = null;
        if (model.CoverImage != null && model.CoverImage.Length > 0)
        {
            string coversFolder = Path.Combine(_env.WebRootPath, "uploads", "covers");
            Directory.CreateDirectory(coversFolder);

            string coverFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.CoverImage.FileName)}";
            string coverFilePath = Path.Combine(coversFolder, coverFileName);
            using (var stream = new FileStream(coverFilePath, FileMode.Create))
            {
                await model.CoverImage.CopyToAsync(stream);
            }
            coverPath = $"/uploads/covers/{coverFileName}";
        }

        // -------- บันทึกข้อมูลลง Database --------
        var product = new ProductData
        {
            ProductName        = model.ProductName,
            ProductDescription = model.ProductDescription,
            Price              = model.Price,
            FilePath           = $"/uploads/products/{productFileName}",
            CoverPicture       = coverPath,
            SellerId           = sellerId,
            CategoryId         = model.CategoryId,
            Status             = model.Status,
            CreatedDate        = DateTime.Now,
            UpdatedDate        = DateTime.Now
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"อัพโหลดสินค้า \"{model.ProductName}\" สำเร็จ!";
        return RedirectToAction("UploadProduct");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
