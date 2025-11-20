using HOMEnitor.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36))
    ));

// Razor Pages
builder.Services.AddRazorPages();

// Session configuration (permanent, no timeout)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = System.TimeSpan.MaxValue; // practically permanent
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Middleware
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // Must come before MapRazorPages
app.UseAuthorization();

app.MapRazorPages();
app.Run();
