using EventApp.Models;
using EventApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EventApp.Controllers
{
    public class HomeController : Controller
    {
        // Hardcoded username and password for authentication
        private const string Username = "admin";

        private const string Password = "password123";

        // GET: Show the login page
        public IActionResult Login()
        {
            return View();
        }

        // POST: Handle login submission
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Validate hardcoded credentials
                if (model.Username == Username && model.Password == Password)
                {
                    // Set session to indicate user is logged in
                    HttpContext.Session.SetString("User", Username);
                    return RedirectToAction("Index", "Event");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                }
            }

            return View(model);
        }

        // Logout action
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear session on logout
            return RedirectToAction("Login");
        }
    }
}