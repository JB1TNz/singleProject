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
