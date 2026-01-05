using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using webbandt.Models;

namespace webbandt.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        // Constructor chuẩn bắt buộc phải có cho .NET Core
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<HangSanXuat> HangSanXuats { get; set; }
        public DbSet<Hoadontamthoi> Hoadontamthois { get; set; }
        
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public DbSet<DanhGia> DanhGias { get; set; }
        public DbSet<TraLoiBinhLuan> TraLoiBinhLuans { get; set; }
    }
}