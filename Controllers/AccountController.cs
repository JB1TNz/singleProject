using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using singleProject.Models;

namespace singleProject.Controllers;

public class AccountController : Controller
{
  public IActionResult Login()
  {
    return View();
  }

  public IActionResult UserLists()
  {
    return View();
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
  {
    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
  }
}