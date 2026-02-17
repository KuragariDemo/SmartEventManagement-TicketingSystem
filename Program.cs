using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartEventManagement_TicketingSystem.Data;
using SmartEventManagement_TicketingSystem.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// Database Connection
// ===============================
var connectionString = builder.Configuration.GetConnectionString(
    "SmartEventManagement_TicketingSystemContextConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<SmartEventManagement_TicketingSystemContext>(options =>
    options.UseSqlServer(connectionString));

// ===============================
// Identity Configuration
// ===============================
builder.Services
    .AddDefaultIdentity<SmartEventManagement_TicketingSystemUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;

        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole>() // Enable Roles
    .AddEntityFrameworkStores<SmartEventManagement_TicketingSystemContext>();

// ===============================
// MVC + Razor Pages
// ===============================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// ===============================
// Middleware Pipeline
// ===============================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ===============================
// ROLE + ADMIN SEEDING
// ===============================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<SmartEventManagement_TicketingSystemUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Create Roles if not exist
    string[] roles = { "Admin", "Member" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Admin credentials
    string adminEmail = "admin@admin.com";
    string adminPassword = "Admin123";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var user = new SmartEventManagement_TicketingSystemUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "System Administrator",
            PhoneNumber = "0000000000",
            City = "Admin City",
            Bio = "Main Administrator Account",
            IsMember = true // ✅ Admin is automatically treated as member
        };

        var result = await userManager.CreateAsync(user, adminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
    else
    {
        // Ensure admin always has Admin role
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ===============================
// Routing
// ===============================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
