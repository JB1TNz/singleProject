using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using singleProject.Models;
using singleProject.ViewModels;
using singleProject.Models.Db;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

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

      if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
      {
          string hashedPass = "";
          using (SHA1 sha1 = SHA1.Create())
          {
              byte[] bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(pass));
              hashedPass = Convert.ToBase64String(bytes);
          }

          using (var db = new EbookBestContext())
          {
              // เช็คทั้ง Username หรือ Email ก็ได้ เพราะฟอร์มบอกให้ใส่ Email แต่ bind ข้อมูลเป็น Username
              var dbUser = db.UserData.FirstOrDefault(u => 
                  (u.UserName == user || u.Email == user) && 
                  u.UserPassword == hashedPass);

              if (dbUser != null)
              {
                  HttpContext.Session.SetString("Role", dbUser.UserRole ?? "User");
                  HttpContext.Session.SetString("Username", dbUser.UserName ?? user);
                  return RedirectToAction("Index", "Home");
              }
          }
      }

      ViewBag.ErrorMessage = "User is Incorrect";
      return View(data);
  }

  [HttpPost]
  public IActionResult Register(UserDatum data)
  {
      ModelState.Remove("UserId");
      ModelState.Remove("UserRole");
      
      if (ModelState.IsValid)
      {
          // สร้าง UserId (ความยาวสูงสุดใน DB คือ 10)
          data.UserId = Guid.NewGuid().ToString().Substring(0, 10);
          
          // แปลงรหัสผ่านเป็น Hash (ใช้ SHA1 แปลงเป็น Base64 ได้ความยาว 28 ตัวอักษร เพื่อไม่เกิน 30 ของ Database)
          if (!string.IsNullOrEmpty(data.UserPassword))
          {
              using (SHA1 sha1 = SHA1.Create())
              {
                  byte[] bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(data.UserPassword));
                  data.UserPassword = Convert.ToBase64String(bytes);
              }
          }
          
          data.UserRole = "User"; // กำหนดบทบาทพื้นฐาน

          // นำข้อมูลไปเก็บใน Database
          using (var db = new EbookBestContext())
          {
              db.UserData.Add(data);
              db.SaveChanges();
          }
          
          return RedirectToAction("Login");
      }
      return RedirectToAction("Login");
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