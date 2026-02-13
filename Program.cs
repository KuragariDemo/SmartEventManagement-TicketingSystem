using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartEventManagement_TicketingSystem.Data;
using SmartEventManagement_TicketingSystem.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// Database Connection
var connectionString = builder.Configuration.GetConnectionString(
    "SmartEventManagement_TicketingSystemContextConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<SmartEventManagement_TicketingSystemContext>(options =>
    options.UseSqlServer(connectionString));

// Identity Configuration
builder.Services
    .AddDefaultIdentity<SmartEventManagement_TicketingSystemUser>(options =>
    {
        // Allow login immediately after register
        options.SignIn.RequireConfirmedAccount = false;

        // Password rules
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole>() // Enable Roles
    .AddEntityFrameworkStores<SmartEventManagement_TicketingSystemContext>();

// MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Build App
var app = builder.Build();

// Middleware Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Identity
app.UseAuthentication();
app.UseAuthorization();

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Identity Razor Pages
app.MapRazorPages();

app.Run();
