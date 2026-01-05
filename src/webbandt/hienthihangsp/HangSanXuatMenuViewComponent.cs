using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webbandt.Models;
using webbandt.Data; // <--- Bắt buộc có dòng này

namespace webbandt.hienthihangsp
{
    public class HangSanXuatMenuViewComponent : ViewComponent
    {
        // Sửa thành ApplicationDbContext
        private readonly ApplicationDbContext _context;

        public HangSanXuatMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await _context.HangSanXuats.OrderBy(x => x.TenHang).ToListAsync();
            return View(items);
        }
    }
}