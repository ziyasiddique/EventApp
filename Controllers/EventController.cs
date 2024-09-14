using EventApp.Data;
using EventApp.Models;
using EventApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class EventController : Controller
{
    private readonly EventDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<EventController> logger;

    public EventController(EventDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<EventController> logger)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        this.logger = logger;
    }

    // GET: Event/Index
    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("User") == null)
        {
            return RedirectToAction("Login", "Home");
        }

        var events = await _context.Events.ToListAsync();
        return View(events);
    }

    // GET: AddEvent
    public IActionResult AddEvent()
    {
        return View();
    }

    // POST: AddEvent
    [HttpPost]
    public async Task<IActionResult> AddEvent(EventViewModel model)
    {
        if (model.OptionalImages != null && model.OptionalImages.Any())
        {
            foreach (var optionalImage in model.OptionalImages)
            {
                Console.WriteLine($"Optional Image Found: {optionalImage?.FileName}");
            }
        }
        else
        {
            Console.WriteLine("No Optional Images Found");
        }

        if (ModelState.IsValid)
        {
            var eventEntity = new Event
            {
                Title = model.Title,
                Description = model.Description
            };

            // Handle main image upload
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

            // Handle optional images upload
            if (model.OptionalImages != null && model.OptionalImages.Any())
            {
                eventEntity.OptionalImages = new List<OptionalImage>();

                foreach (var optionalImage in model.OptionalImages)
                {
                    if (optionalImage != null)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "optional");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + optionalImage.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await optionalImage.CopyToAsync(fileStream);
                        }
                        eventEntity.OptionalImages.Add(new OptionalImage
                        {
                            ImagePath = "/images/optional/" + uniqueFileName,
                            EventId = eventEntity.Id
                        });

                        // Log the file path
                        Console.WriteLine($"Optional Image Path: /images/optional/{uniqueFileName}");
                    }
                }
            }

            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    // GET: Event/Details
    public async Task<IActionResult> Details(int id)
    {
        var eventEntity = await _context.Events
            .Include(e => e.OptionalImages) // Include the related OptionalImages
            .AsNoTracking()
            .SingleOrDefaultAsync(e => e.Id == id);

        if (eventEntity == null)
        {
            return NotFound();
        }

        var viewModel = new EventDetailsViewModel
        {
            Event = eventEntity,
            OptionalImages = eventEntity.OptionalImages
        };

        return View(viewModel);
    }

    // GET: Edit
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

    // POST: Edit
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

            // Update title and description if changed
            if (!string.IsNullOrWhiteSpace(model.Title))
            {
                eventEntity.Title = model.Title;
            }

            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                eventEntity.Description = model.Description;
            }

            // Handle image upload if a new image is provided
            if (model.Image != null)
            {
                // Delete old image if it exists
                if (!string.IsNullOrEmpty(eventEntity.ImagePath))
                {
                    string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, eventEntity.ImagePath.TrimStart('/'));

                    // Make sure the file exists before attempting to delete it
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Upload new image
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(fileStream);
                }

                // Update the image path in the entity
                eventEntity.ImagePath = "/images/" + uniqueFileName;
            }

            // Handle optional images upload
            if (model.OptionalImages != null && model.OptionalImages.Any())
            {
                foreach (var optionalImage in model.OptionalImages)
                {
                    if (optionalImage != null)
                    {
                        string optionalUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "optional");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + optionalImage.FileName;
                        string optionalFilePath = Path.Combine(optionalUploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(optionalFilePath, FileMode.Create))
                        {
                            await optionalImage.CopyToAsync(fileStream);
                        }

                        var newOptionalImage = new OptionalImage
                        {
                            ImagePath = "/images/optional/" + uniqueFileName,
                            EventId = eventEntity.Id // Foreign key to Event
                        };

                        _context.OptionalImages.Add(newOptionalImage);
                    }
                }
            }

            // Update the event entity in the database
            _context.Update(eventEntity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    // POST: Delete
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var eventEntity = await _context.Events.FindAsync(id);
        if (eventEntity == null)
        {
            return NotFound();
        }

        // Remove associated optional images
        var optionalImages = _context.OptionalImages.Where(i => i.EventId == id).ToList();
        _context.OptionalImages.RemoveRange(optionalImages);

        _context.Events.Remove(eventEntity);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}