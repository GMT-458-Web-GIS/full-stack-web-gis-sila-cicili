using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies; // Gerekli kütüphane

var builder = WebApplication.CreateBuilder(args);

// Tarih ayarı (PostgreSQL hatası için)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddControllersWithViews();

// 1. GİRİŞ SİSTEMİ AYARI (COOKIE)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Giriş yapmamış kişiyi buraya at
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // 20 dakika sonra at
    });

// Veritabanı Bağlantısı
builder.Services.AddDbContext<KütüphaneeContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LibraryContext")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 2. BU İKİ SATIR ÇOK ÖNEMLİ (SIRASI BOZULMAMALI)
app.UseAuthentication(); // Kimlik Kontrolü (Kimsin?)
app.UseAuthorization();  // Yetki Kontrolü (Girebilir misin?)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();