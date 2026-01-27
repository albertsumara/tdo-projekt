using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CarRentalApp.Areas.Identity.Data;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Sprawdzamy, czy aplikacja działa na chmurze Render
var isRender = Environment.GetEnvironmentVariable("RENDER") != null;

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (isRender)
    {
        // Konfiguracja dla Render (Online) - używamy SQLite
        options.UseSqlite("Data Source=carrental.db");
    }
    else
    {
        // Konfiguracja lokalna (Docker Compose) - używamy PostgreSQL
        var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection")
            ?? throw new InvalidOperationException("Connection string 'AppDbContextConnection' not found.");
        options.UseNpgsql(connectionString);
    }
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Automatyczne migracje i inicjalizacja bazy
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try 
    {
        var context = services.GetRequiredService<AppDbContext>();
        if (isRender)
        {
            // Na SQLite używamy EnsureCreated zamiast Migrate, by uniknąć problemów z historią migracji z Postgresa
            await context.Database.EnsureCreatedAsync();
        }
        else 
        {
            await context.Database.MigrateAsync();
        }
        await CarRentalApp.Data.DbInitializer.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Wystąpił błąd podczas inicjalizacji bazy danych.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Render wymaga nasłuchiwania na porcie 8080 lub pobranego z ENV
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.UseHttpMetrics();
app.UseSession();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapMetrics(); 
app.MapRazorPages();

app.Run();