using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using webbandt.Data;

var builder = WebApplication.CreateBuilder(args);

// Kết nối csdl
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ============================================================
// [MỚI 1] ĐĂNG KÝ DỊCH VỤ SESSION
// ============================================================
// Cần thêm Cache vì Session lưu dữ liệu vào RAM của server
builder.Services.AddDistributedMemoryCache();

// Cấu hình Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Giỏ hàng sẽ tự hủy sau 30 phút nếu không thao tác
    options.Cookie.HttpOnly = true; // Bảo mật: JavaScript không thể can thiệp vào cookie này
    options.Cookie.IsEssential = true; // Bắt buộc ghi cookie ngay cả khi chưa chấp nhận chính sách cookie
});
// ============================================================


builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    // 1. Cấu hình để mật khẩu dễ chịu hơn (Tùy chọn)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.SignIn.RequireConfirmedAccount = false;

})
    .AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddErrorDescriber<webbandt.VietnameseIdentityErrorDescriber>();

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmailSender, EmailSender>();
var app = builder.Build();

// Cấu hình đường dẫn yêu cầu http 
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ============================================================
// [MỚI 2] KÍCH HOẠT SESSION MIDDLEWARE
// Quan trọng: Phải đặt SAU UseRouting và TRƯỚC UseAuthorization
// ============================================================
app.UseSession();
// ============================================================

app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
