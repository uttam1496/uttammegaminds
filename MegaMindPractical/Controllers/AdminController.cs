using MegaMindPractical.Models;
using Microsoft.AspNetCore.Mvc;

namespace MegaMindPractical.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        public IActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            return View(_context.Users.ToList());
        }

        public IActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(User user, int categoryId)
        {
            user.IsAdmin = Request.Form["IsAdmin"] == "true";

            if (string.IsNullOrEmpty(user.Name) ||
                string.IsNullOrEmpty(user.Email) ||
                string.IsNullOrEmpty(user.Password))
            {
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Error = "All fields required";
                return View(user);
            }

            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();

                _context.UserCategories.Add(new UserCategory
                {
                    UserId = user.Id,
                    CategoryId = categoryId
                });

                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Categories = _context.Categories.ToList();
                return View(user);
            }
        }

        // ✅ GET EDIT
        public IActionResult Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var user = _context.Users.Find(id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // ✅ FIXED POST EDIT
        [HttpPost]
        public IActionResult Edit(User user)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var existingUser = _context.Users.Find(user.Id);

            if (existingUser == null)
                return NotFound();

            // update manually
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Password = user.Password;
            existingUser.Phone = user.Phone;
            existingUser.IsAdmin = Request.Form["IsAdmin"] == "true";

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var user = _context.Users.Find(id);

            if (user != null)
            {
                var mappings = _context.UserCategories
                    .Where(x => x.UserId == id)
                    .ToList();

                _context.UserCategories.RemoveRange(mappings);
                _context.Users.Remove(user);

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public JsonResult GetSubCategory(int id)
        {
            var data = _context.SubCategories
                .Where(x => x.CategoryId == id)
                .Select(x => new
                {
                    id = x.Id,
                    name = x.Name
                })
                .ToList();

            return Json(data);
        }
    }
}