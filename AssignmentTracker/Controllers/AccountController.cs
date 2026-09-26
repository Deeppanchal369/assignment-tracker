using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssignmentTracker.Data;
using AssignmentTracker.Models.ViewModels;

namespace AssignmentTracker.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Already logged in? Send to the correct dashboard.
            var role = HttpContext.Session.GetString("UserRole");
            if (role == "Teacher") return RedirectToAction("Dashboard", "Teacher");
            if (role == "Student") return RedirectToAction("Dashboard", "Student");

            return View(new LoginViewModel { Role = "Student" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Role == "Teacher")
            {
                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => t.Email == model.Email);

                if (teacher == null || !PasswordHelper.Verify(model.Password, teacher.Password))
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(model);
                }

                HttpContext.Session.SetInt32("UserId", teacher.TeacherId);
                HttpContext.Session.SetString("UserRole", "Teacher");
                HttpContext.Session.SetString("UserName", teacher.Name);

                return RedirectToAction("Dashboard", "Teacher");
            }
            else
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.Email == model.Email);

                if (student == null || !PasswordHelper.Verify(model.Password, student.Password))
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(model);
                }

                HttpContext.Session.SetInt32("UserId", student.StudentId);
                HttpContext.Session.SetString("UserRole", "Student");
                HttpContext.Session.SetString("UserName", student.Name);

                return RedirectToAction("Dashboard", "Student");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
