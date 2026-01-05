using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using webbandt.Models;
using webbandt.Data;

namespace webbandt.Controllers
{
    public class Kiemtrathanhtoan : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public Kiemtrathanhtoan(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // 1. Trang Hóa Đơn Tạm Thời (Checkout)
        public IActionResult Index()
        {
            var cart = GetCartFromSession();
            if (cart.Count == 0) return RedirectToAction("Index", "Home");

            var model = new Hoadontamthoi
            {
                GioHang = cart,
                TongTien = cart.Sum(x => x.ThanhTien)
            };
            return View(model);
        }

        // 2. Xử lý khi bấm nút "Mua Ngay" từ trang chủ
        [HttpPost]
        public IActionResult ThemVaoGio(int id)
        {
            // Tìm sản phẩm từ DB để đảm bảo giá đúng
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(p => p.ProductId == id);

            if (item != null)
            {
                item.SoLuong++;
            }
            else
            {
                cart.Add(new Sanphamtronggiohang
                {
                    ProductId = product.Id,
                    TenSanPham = product.Name,
                    DonGia = product.Price,
                    HinhAnh = product.ImageUrl,
                    SoLuong = 1
                });
            }

            // Lưu lại vào Session
            HttpContext.Session.SetString("GioHang", JsonSerializer.Serialize(cart));

            // Chuyển hướng đến trang thanh toán
            return RedirectToAction("GioHang", "Home");
        }

        // 3. Xử lý Lưu Đơn Hàng (SQL Thuần - ADO.NET)
        [HttpPost]
        public IActionResult ProcessPayment(Hoadontamthoi input)
        {
            var cart = GetCartFromSession();
            if (cart.Count == 0) return RedirectToAction("Index", "Home");

            input.TongTien = cart.Sum(x => x.ThanhTien);

            // ==================================================================
            // [QUAN TRỌNG] Tự động gán Email nếu khách đang đăng nhập
            // ==================================================================
            if (User.Identity?.IsAuthenticated == true)
            {
                // Lấy Email từ tài khoản đăng nhập gán đè vào input
                // Thêm 2 dấu hỏi và cặp ngoặc kép vào cuối
                input.Email = User.Identity?.Name ?? "";
            }
            // ==================================================================

            string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            // Lưu bảng DonHang
                            string sqlOrder = @"INSERT INTO DonHang (HoTen, SoDienThoai, Email, DiaChi, GhiChu, TongTien) 
                                                VALUES (@ten, @sdt, @mail, @diachi, @note, @tien);
                                                SELECT CAST(SCOPE_IDENTITY() as int);";

                            int orderId;
                            using (SqlCommand cmd = new SqlCommand(sqlOrder, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@ten", input.HoTen);
                                cmd.Parameters.AddWithValue("@sdt", input.SoDienThoai);
                                // Dùng input.Email (đã được gán tự động ở trên nếu có đăng nhập)
                                cmd.Parameters.AddWithValue("@mail", input.Email ?? "");
                                cmd.Parameters.AddWithValue("@diachi", input.DiaChi);
                                cmd.Parameters.AddWithValue("@note", input.GhiChu ?? "");
                                cmd.Parameters.AddWithValue("@tien", input.TongTien);
                                orderId = (int)cmd.ExecuteScalar();
                            }

                            // Lưu bảng ChiTietDonHang
                            string sqlDetail = @"INSERT INTO ChiTietDonHang (DonHangId, TenSanPham, KichThuoc, SoLuong, DonGia) 
                                                 VALUES (@did, @dten, @dsize, @dqty, @dgia)";

                            foreach (var item in cart)
                            {
                                using (SqlCommand cmdDetail = new SqlCommand(sqlDetail, conn, trans))
                                {
                                    cmdDetail.Parameters.AddWithValue("@did", orderId);
                                    cmdDetail.Parameters.AddWithValue("@dten", item.TenSanPham);
                                    cmdDetail.Parameters.AddWithValue("@dsize", item.KichThuoc ?? "Tiêu chuẩn");
                                    cmdDetail.Parameters.AddWithValue("@dqty", item.SoLuong);
                                    cmdDetail.Parameters.AddWithValue("@dgia", item.DonGia);
                                    cmdDetail.ExecuteNonQuery();
                                }
                            }

                            trans.Commit();
                            HttpContext.Session.Remove("GioHang");

                            return Content($"<script>alert('Đặt hàng thành công! Mã đơn: #{orderId}'); window.location.href='/';</script>", "text/html");
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("Lỗi đặt hàng: " + ex.Message);
            }
        }

        private List<Sanphamtronggiohang> GetCartFromSession()
        {
            var session = HttpContext.Session.GetString("GioHang");
            if (string.IsNullOrEmpty(session))
            {
                return new List<Sanphamtronggiohang>();
            }
            return JsonSerializer.Deserialize<List<Sanphamtronggiohang>>(session) ?? new List<Sanphamtronggiohang>();
        }
    }
}