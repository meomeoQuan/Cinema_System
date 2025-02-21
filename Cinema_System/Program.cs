using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Cinema.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(u => u.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("DefaultConnection"),
//        b => b.MigrationsAssembly("Cinema.DataAccess") // ✅ Chỉ định dự án chứa Migration
//    ));

builder.Services.AddRazorPages();
builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
//options => options.SignIn.RequireConfirmedAccount = true
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
//app.UseAuthentication();
//app.UseAuthorization();
app.MapRazorPages();
app.MapStaticAssets();




// ✅ Seed dữ liệu khi ứng dụng khởi động
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>(); // 🛠 Được đăng ký đúng
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await DbInitializer.SeedUsers(userManager, roleManager); // Gọi SeedUsers
}
// ------------------- ROUTING CHO AREAS ------------------- //

// 1) Route cho Admin
//    Khi URL bắt đầu bằng "/Admin/...",
//    MVC sẽ tìm controller trong Area = "Admin".
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{area=Guest}/{controller=Home}/{action=Index}/{id?}")
//    .WithStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Admin}/{controller=Users}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
//app.Use(async (context, next) =>
//{
//    if (context.Request.Path.StartsWithSegments("/Admin"))
//    {
//        var user = new System.Security.Claims.ClaimsPrincipal(
//            new System.Security.Claims.ClaimsIdentity(
//                new[]
//                {
//                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "AdminTest"),
//                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Admin"),
//                }, "TestAuth"
//            )
//        );
//        context.User = user;
//    }
//    await next();
//});
//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.ExpireTimeSpan = TimeSpan.FromDays(30); // Lưu đăng nhập 30 ngày
//    options.SlidingExpiration = true;
//});
