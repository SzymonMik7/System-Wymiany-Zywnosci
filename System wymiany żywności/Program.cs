using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System_wymiany_żywności.Data;
using System_wymiany_żywności.Models;
using System_wymiany_żywności.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Połączenie z bazą danych
// Pobieramy "Connection String" z pliku appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

// Konfigurujemy Entity Framework, aby używał bazy SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Konfiguracja systemu Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>() // Powiązanie Identity z naszą bazą danych
.AddDefaultUI()
.AddDefaultTokenProviders();

// 3. Rejestracja własnych serwisów
// Rejestrujemy IExchangeService, aby móc go wstrzykiwać do kontrolerów
builder.Services.AddScoped<IExchangeService, ExchangeService>();

// Obsługa stron Razor Pages
builder.Services.AddRazorPages();

// Obsługa kontrolerów i widoków MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();


// Pozwala aplikacji na serwowanie plików statycznych
app.UseStaticFiles();

// Dopasowywania adresów URL do kontrolerów
app.UseRouting();

// Rozpoznawanie "kto jest zalogowany"
app.UseAuthentication();

// Mechanizm sprawdzania możliwości użytkownika
app.UseAuthorization();

// Mapowanie ścieżek dla systemu 
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();