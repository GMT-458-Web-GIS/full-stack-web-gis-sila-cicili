using LibrarySystem.Services;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Tarih ayarı
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddControllersWithViews();

// 1. GİRİŞ SİSTEMİ
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

// 2. VERİTABANI BAĞLANTISI (HARİTA AYARIYLA BERABER) 🌍
builder.Services.AddDbContext<KütüphaneeContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LibraryContext"), 
        o => o.UseNetTopologySuite())); // ⚠️ BU SATIR HAYAT KURTARIR

// Servisler
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
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