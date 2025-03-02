using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Cinema.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Cinema.Utility;
using Cinema.DbInitializer;
using Cinema.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


//builder.Services.AddDbContext<ApplicationDbContext>(u => u.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("DefaultConnection"),
//        b => b.MigrationsAssembly("Cinema.DataAccess") // ✅ Chỉ định dự án chứa Migration
//    ));


//---------------------------------------------------------------------------------------
// Configure database context

//-------------------------------------- SIGNAL IR   -------------------------------------------------


builder.Services.AddSignalR();

//---------------------------------------------------------------------------------------

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Configure Identity with ApplicationUser (fixing UserManager<IdentityUser> issue)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure Identity lockout policy
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;
});

builder.Services.AddRazorPages();

// Configure authentication cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

//// Add Facebook authentication
//builder.Services.AddAuthentication().AddFacebook(options =>
//{
//    options.AppId = "572726168935390";
//    options.AppSecret = "ef269c0c3efbd79bfae81afdcba26300";
//});

// Add Google authentication
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "1090292520927-n8hcmp4v0f4u1peg91j9mdadadjdl72u.apps.googleusercontent.com";
    options.ClientSecret = "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";
});

//---------------------------------------------------------------------------------------
// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure token lifespan
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromSeconds(30);
});

//---------------------------------------------------------------------------------------
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

//----------------------------------------------------------------------------------------
var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();  // Ensure static files are served
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Seed database
SeedDatabase();

app.MapRazorPages();
app.MapStaticAssets();

app.MapHub<ChatHub>("/chatHub");
// ------------------- ROUTING CHO AREAS ------------------- //
// 1) Route cho Admin
//    Khi URL bắt đầu bằng "/Admin/...",
//    MVC sẽ tìm controller trong Area = "Admin".
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Guest}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{area=Admin}/{controller=Users}/{action=Index}/{id?}")
//    .WithStaticAssets();

app.Run();

void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var DbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        DbInitializer.Initialize();
    }
}
