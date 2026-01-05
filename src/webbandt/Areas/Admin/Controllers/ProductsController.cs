using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webbandt.Data;
using webbandt.Models;
using Microsoft.AspNetCore.Authorization;

namespace webbandt.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnv; // 1. Khai báo trợ lý môi trường

        // 2. Sửa hàm khởi tạo để nhận trợ lý vào (thêm tham số thứ 2)
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment hostEnv)
        {
            _context = context;
            _hostEnv = hostEnv;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["MaHang"] = new SelectList(_context.HangSanXuats, "MaHang", "TenHang");
            return View();
        }

        /// POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Thêm tham số imageFile để nhận ảnh từ form
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Description,SoLuongTon,MaHang,ManHinh,HeDieuHanh,Ram,DungLuong,Pin,CameraSau,CameraTruoc,Chip")] Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // --- BẮT ĐẦU XỬ LÝ ẢNH ---
                if (imageFile != null && imageFile.Length > 0)
                {
                    // 1. Tạo tên file mới để tránh trùng (Ví dụ: iphone.jpg -> iphone_123456789.jpg)
                    string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                    string extension = Path.GetExtension(imageFile.FileName);
                    fileName = fileName + "_" + DateTime.Now.Ticks.ToString() + extension;

                    // 2. Xác định đường dẫn lưu ảnh vào thư mục wwwroot/images
                    string path = Path.Combine(_hostEnv.WebRootPath, "images", fileName);

                    // 3. Copy ảnh vào đường dẫn đó
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    // 4. Lưu đường dẫn vào đối tượng Product để nạp vào Database
                    product.ImageUrl = "/images/" + fileName;
                }
                else
                {
                    // Nếu không chọn ảnh thì gán ảnh mặc định (nếu muốn)
                    product.ImageUrl = "/images/default.jpg";
                }
                // --- KẾT THÚC XỬ LÝ ẢNH ---

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,SoLuongTon,ImageUrl,MaHang,Status,ManHinh,HeDieuHanh,CameraTruoc,CameraSau,Chip,Ram,DungLuong,Pin")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
