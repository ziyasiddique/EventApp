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

        // GET: Details action to view event details
        public async Task<IActionResult> Details(int id)
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity == null)
            {
                return NotFound();
            }
            return View(eventEntity);
        }

        // Controllers/EventController.cs

        // GET: Edit action to show the edit form
        public async Task<IActionResult> Edit(int id)
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity == null)
            {
                return NotFound();
            }

            var viewModel = new EventViewModel
            {
                Id = eventEntity.Id,
                Title = eventEntity.Title,
                Description = eventEntity.Description,
                // No need to set Image if it's optional
            };

            return View(viewModel);
        }

        // POST: Edit action to handle form submission
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EventViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var eventEntity = await _context.Events.FindAsync(id);
                if (eventEntity == null)
                {
                    return NotFound();
                }

                // Update title if changed
                if (!string.IsNullOrWhiteSpace(model.Title))
                {
                    eventEntity.Title = model.Title;
                }

                // Update description if changed
                if (!string.IsNullOrWhiteSpace(model.Description))
                {
                    eventEntity.Description = model.Description;
                }

                // Handle image upload if a new image is provided
                if (model.Image != null)
                {
                    // Delete the old image file if needed
                    if (!string.IsNullOrEmpty(eventEntity.ImagePath))
                    {
                        string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, eventEntity.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save the new image
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fileStream);
                    }
                    eventEntity.ImagePath = "/images/" + uniqueFileName;
                }

                _context.Update(eventEntity);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // POST: Delete action to delete an event
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity == null)
            {
                return NotFound();
            }

            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}