using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CarRentalApp.Areas.Identity.Data;
using Prometheus;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AppDbContextConnection' not found.");;

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>() .AddEntityFrameworkStores<AppDbContext>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options=> options.UseSqlite(builder.Configuration.GetConnectionString("AppDbContextConnection")));
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>(); // Pobierz kontekst bazy
    context.Database.EnsureCreated(); // To stworzy bazę i tabele, jeśli ich nie ma

    await CarRentalApp.Models.TworzenieAdmina.InicjalizujRole(services);
    await CarRentalApp.Models.TworzenieAdmina.InicjalizujKategorie(services);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRouting();

app.UseHttpMetrics();

app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapMetrics(); 
app.MapRazorPages();

app.Run();
