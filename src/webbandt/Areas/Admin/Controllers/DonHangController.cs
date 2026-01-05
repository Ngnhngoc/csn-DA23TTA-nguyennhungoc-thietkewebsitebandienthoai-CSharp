using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webbandt.Data;
using webbandt.Models; // Thay bằng namespace models của bạn

namespace webbandt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. HIỂN THỊ DANH SÁCH ĐƠN HÀNG
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách đơn hàng, sắp xếp mới nhất lên đầu
            var donHangs = await _context.DonHangs
                                         .OrderByDescending(d => d.NgayDat)
                                         .ToListAsync();
            return View(donHangs);
        }

        // 2. XEM CHI TIẾT ĐƠN HÀNG
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Lấy đơn hàng kèm theo chi tiết sản phẩm
            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.Product) // Load thông tin sản phẩm từ chi tiết
                .FirstOrDefaultAsync(m => m.Id == id);

            if (donHang == null) return NotFound();

            return View(donHang);
        }

        // 3. CẬP NHẬT TRẠNG THÁI ĐƠN HÀNG (VD: Đang giao, Đã giao...)
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, int trangThai)
        {
            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang != null)
            {
                donHang.TrangThai = trangThai;
                _context.Update(donHang);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = id });
        }
    }
}