using LibrarySystem.Services;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 1. SERVİS AYARLARI
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddControllersWithViews();

// 👇 SWAGGER EKLENTİSİ
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// GİRİŞ SİSTEMİ
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

// VERİTABANI BAĞLANTISI
// Not: Burada 'LibraryContext' yazması doğru, çünkü ConnectionString ismin bu.
builder.Services.AddDbContext<KütüphaneeContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LibraryContext"), 
        o => o.UseNetTopologySuite())); 

// Kendi Servislerin
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

// --- 🔥 SİHİRLİ KOD BURASI: TABLOLARI OLUŞTURUYOR ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Senin veritabanı sınıfın 'KütüphaneeContext' olduğu için bunu çağırıyoruz
        var context = services.GetRequiredService<KütüphaneeContext>();
        
        // Bu komut, veritabanı boşsa tabloları (Books, Users vb.) otomatik oluşturur
        context.Database.Migrate(); 
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Tablolar oluşturulurken bir hata oluştu.");
    }
}
// --- SİHİRLİ KOD BİTTİ ---

// 2. MIDDLEWARE AYARLARI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();