using MegaMindPractical.Models;
using Microsoft.AspNetCore.Mvc;

namespace MegaMindPractical.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.Email == email && x.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Role", user.IsAdmin ? "Admin" : "User");

                if (user.IsAdmin)
                    return RedirectToAction("Index", "Admin");

                return RedirectToAction("Dashboard", "User");
            }

            ViewBag.Error = "Invalid Login";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
