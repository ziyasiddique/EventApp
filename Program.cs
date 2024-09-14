using EventApp.Data;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();  // For logging to the console
builder.Logging.AddDebug();
// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext with SQL Server
builder.Services.AddDbContext<EventDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Session timeout
    options.Cookie.HttpOnly = true;  // Security measure
    options.Cookie.IsEssential = true;  // Required for session to work without explicit consent
});

// Configure FormOptions to handle large file uploads (if necessary)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue; // Adjust as necessary
    Console.WriteLine($"File size limit set to: {options.MultipartBodyLengthLimit}");
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session middleware
app.UseSession();

app.UseAuthorization();

// Set default routing to Login page
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();