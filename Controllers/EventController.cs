using EventApp.Data;
using EventApp.Models;
using EventApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EventApp.Controllers
{
    public class EventController : Controller
    {
        private readonly EventDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EventController(EventDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Index action to list all events
        public IActionResult Index()
        {
            var events = _context.Events.ToList();
            return View(events);
        }

        // GET: AddEvent action to show the form
        public IActionResult AddEvent()
        {
            return View();
        }

        // POST: AddEvent action to handle form submission
        [HttpPost]
        public async Task<IActionResult> AddEvent(EventViewModel model)
        {
            if (ModelState.IsValid)
            {
                var eventEntity = new Event
                {
                    Title = model.Title,
                    Description = model.Description
                };

                // Handle image upload
                if (model.Image != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fileStream);
                    }
                    eventEntity.ImagePath = "/images/" + uniqueFileName;
                }

                // Save event to the database
                _context.Events.Add(eventEntity);
                await _context.SaveChangesAsync();

                // Redirect to the Index page
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}