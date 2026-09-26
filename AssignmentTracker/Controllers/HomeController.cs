using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AssignmentTracker.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // If already logged in, redirect straight to the right dashboard.
            var role = HttpContext.Session.GetString("UserRole");
            if (role == "Teacher")
            {
                return RedirectToAction("Dashboard", "Teacher");
            }
            if (role == "Student")
            {
                return RedirectToAction("Dashboard", "Student");
            }

            return RedirectToAction("Login", "Account");
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
