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

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Supporter()
    {
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
