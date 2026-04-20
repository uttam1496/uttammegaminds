using MegaMindPractical.Models;
using Microsoft.AspNetCore.Mvc;

namespace MegaMindPractical.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var categoryIds = _context.UserCategories
                .Where(x => x.UserId == userId)
                .Select(x => x.CategoryId)
                .ToList();

            var subCategories = _context.SubCategories
                .Where(x => categoryIds.Contains(x.CategoryId))
                .ToList();

            return View(subCategories);
        }
    }
}