using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using webbandt.Data;
using webbandt.Models;

namespace webbandt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        // 1. Khai báo đúng kiểu dữ liệu (Sửa lỗi CS1061 và CS0649)
        private readonly ApplicationDbContext _context;

        // 2. Thêm Constructor (Sửa lỗi CS0649)
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context; // Gán context
        }

        public IActionResult Index()
        {
            // 1. Đếm tổng số sản phẩm đang có
            ViewBag.SoLuongSanPham = _context.Products.Count();

            // 2. Đếm tổng số đơn hàng
            ViewBag.SoLuongDonHang = _context.DonHangs.Count();

            // 3. Đếm liên hệ (Nếu bạn chưa có bảng LienHes thì tạm thời để số 0 hoặc xóa dòng này)
            // ViewBag.SoLuongLienHe = _context.LienHes.Count(); 
            ViewBag.SoLuongLienHe = 0; // Tạm để 0 để tránh lỗi nếu chưa có bảng

            return View();
        }


    }
}