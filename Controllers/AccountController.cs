using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using singleProject.Models;
using singleProject.ViewModels;

namespace singleProject.Controllers;

public class AccountController : Controller
{
  public IActionResult Login()
  {
    return View();
  }

  [HttpPost]
  public IActionResult Login(LoginViewModel data)
  {
    string user = data.Username;
    string pass = data.Password;
    ViewBag.Username = user;
    ViewBag.Password = pass;
    return RedirectToAction("Index", "Home", new { username = data.Username });
  }

  public IActionResult UserLists(string user, string passw)
  {
    // var user = new LoginViewModel();
    // user.Username = "JohnDoe";
    // user.Password = "Password123";
    // var user = new List<LoginViewModel>
    // {
    //   new LoginViewModel { Username = "JohnDoe", Password = "Password123" },
    //   new LoginViewModel { Username = "JaneSmith", Password = "SecurePass456" },
    //   new LoginViewModel { Username = "AliceJohnson", Password = "MyPassword789" }
    // };
    // return View(user);e
    ViewBag.Username = user;
    ViewBag.Password = passw;
    return View();
  }



  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
  {
    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
  }
}