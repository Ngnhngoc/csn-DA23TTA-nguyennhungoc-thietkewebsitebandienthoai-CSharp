using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webbandt.Data;
using webbandt.Models;

namespace webbandt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HangSanXuatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HangSanXuatController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Kiểm tra xem có lấy được dữ liệu không
            var items = await _context.HangSanXuats.ToListAsync();
            return View(items);
        }

        // Action Tạo mới (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Action Tạo mới (POST)
        [HttpPost]
        // [ValidateAntiForgeryToken]  <-- Đảm bảo dòng này đã bị xóa hoặc comment lại
        public async Task<IActionResult> Create(HangSanXuat model)
        {
            // 1. Kiểm tra xem Controller có nhận được tên hãng bạn nhập không
            if (string.IsNullOrEmpty(model.TenHang))
            {
                // Nếu dòng này hiện ra -> Lỗi do file View (đặt sai name="...")
                return Content("LỖI: Không nhận được Tên Hãng! Vui lòng kiểm tra lại thẻ <input name='TenHang'> bên View.");
            }

            try
            {
                // 2. Thử lưu vào Database
                _context.Add(model);
                await _context.SaveChangesAsync();

                // Nếu lưu thành công -> Quay về trang danh sách
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // 3. Nếu lỗi Database -> Hiện chi tiết lỗi ra màn hình trắng
                return Content("LỖI DATABASE: " + ex.Message + " | Chi tiết: " + ex.InnerException?.Message);
            }
        }
    }
}