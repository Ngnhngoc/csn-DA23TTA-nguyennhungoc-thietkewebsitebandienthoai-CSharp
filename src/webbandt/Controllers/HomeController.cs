using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using webbandt.Data;
using webbandt.Models;

namespace webbandt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        // Chỉ khai báo 1 dòng duy nhất này thôi:
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Thêm "UserManager<IdentityUser> userManager" vào trong ngoặc bên dưới
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager; // Gán giá trị để sử dụng sau này
            _roleManager = roleManager;
        }
        public async Task<IActionResult> LenLamAdmin()
        {
            // 1. Tạo quyền "Admin" trong database nếu chưa có
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // 2. Tìm tài khoản của bạn
            var user = await _userManager.FindByEmailAsync("ngocsummer0309@gmail.com");

            if (user == null)
            {
                return Content("Lỗi: Không tìm thấy tài khoản ngocsummer0309@gmail.com. Bạn hãy Đăng ký tài khoản này trước nhé!");
            }

            // 3. Gán quyền Admin cho user
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                return Content("Đã cấp quyền Admin thành công! Vui lòng Đăng xuất và Đăng nhập lại để có hiệu lực.");
            }

            return Content("Tài khoản này ĐÃ là Admin rồi.");
        }
        public IActionResult Index()
        {
            var products = _context.Products
                           .Include(p => p.HangSanXuat)
                           .OrderByDescending(p => p.Id) // Sắp xếp mới nhất
                           .Take(12) 
                           .ToList();

            return View(products);
        }

        // Action hiển thị sản phẩm với bộ lọc
        // Thêm tham số 'search' và đổi 'mid' thành 'maHang' cho khớp với ViewComponent
        public IActionResult Sanpham(int? maHang, string search, string sortOrder, int page = 1)
        {
            int pageSize = 16; 

            // 1. Query cơ bản kèm thông tin Hãng
            var products = _context.Products.Include(p => p.HangSanXuat).AsQueryable();

            // 2. Lọc theo Hãng (nếu có chọn)
            if (maHang != null)
            {
                products = products.Where(p => p.MaHang == maHang);
            }
            // QUAN TRỌNG: Lưu lại mã hãng để lát nữa View dùng lại cho các nút Sắp xếp/Phân trang
            ViewBag.CurrentMaHang = maHang;

            // 3. Tìm kiếm theo tên (nếu có nhập)
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search));
            }
            ViewBag.CurrentSearch = search; // Lưu lại từ khóa tìm kiếm

            // 4. Sắp xếp dữ liệu
            switch (sortOrder ?? "")
            {
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "newest":
                    products = products.OrderByDescending(p => p.Id);
                    break;
                default:
                    products = products.OrderByDescending(p => p.Id); // Mặc định là mới nhất
                    break;
            }
            ViewBag.CurrentSort = sortOrder; // Lưu lại trạng thái sắp xếp

            // 5. Tính toán phân trang
            int totalItems = products.Count();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // 6. Cắt dữ liệu (Skip & Take)
            var pagedData = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View(pagedData);
        }
        public IActionResult Chitietsp(int id)
        {
            // 1. Tìm sản phẩm theo ID
            var product = _context.Products
                .Include(p => p.HangSanXuat) // Lấy kèm thông tin Hãng
                .Include(p => p.DanhGias)    // Lấy kèm Đánh giá (để hiển thị bình luận)
                .FirstOrDefault(p => p.Id == id);

            // 2. Nếu không tìm thấy thì báo lỗi 404
            if (product == null)
            {
                return NotFound();
            }

            // 3. (Tùy chọn) Tăng lượt xem cho sản phẩm
            // product.LuotXem = (product.LuotXem ?? 0) + 1;
            // _context.Update(product);
            // _context.SaveChanges();

            return View(product);
        }
        [HttpPost]
        public IActionResult GuiTinNhan(string hoTen, string soDienThoai, string email, string noiDung)
        {
            try
            {
                // 1. CẤU HÌNH GMAIL CỦA BẠN (Admin)
                string adminEmail = "ngocsummer0309@gmail.com"; // <-- Thay Email của bạn vào đây
                string password = "omfg cckc nyno alqa";    // <-- Thay Mật khẩu ứng dụng vào đây

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(adminEmail, password),
                    EnableSsl = true,
                };

                // -----------------------------------------------------------
                // 2. GỬI MAIL CHO ADMIN (Báo có khách liên hệ)
                // -----------------------------------------------------------
                var mailToAdmin = new MailMessage
                {
                    From = new MailAddress(adminEmail, "Hệ Thống Web"),
                    Subject = $"[Khách Mới] {hoTen} vừa liên hệ",
                    Body = $"<p><b>Khách hàng:</b> {hoTen}</p>" +
                           $"<p><b>SĐT:</b> {soDienThoai}</p>" +
                           $"<p><b>Email:</b> {email}</p>" +
                           $"<p><b>Nội dung:</b> {noiDung}</p>",
                    IsBodyHtml = true,
                };
                mailToAdmin.To.Add(adminEmail); // Gửi về cho chính mình
                smtpClient.Send(mailToAdmin);

                // -----------------------------------------------------------
                // 3. GỬI MAIL CHO KHÁCH HÀNG (Quan trọng - Đây là đoạn bạn cần)
                // -----------------------------------------------------------
                if (!string.IsNullOrEmpty(email)) // Kiểm tra xem khách có nhập email không
                {
                    var mailToCustomer = new MailMessage
                    {
                        From = new MailAddress(adminEmail, "Cửa Hàng Điện Thoại PhoneReal"), // Tên hiển thị
                        Subject = "Cảm ơn bạn đã liên hệ!", // Tiêu đề thư khách nhận được
                        Body = $"<p>Chào <b>{hoTen}</b>,</p>" +
                               $"<p>Cảm ơn bạn đã gửi tin nhắn cho chúng tôi.</p>" +
                               $"<p>Chúng tôi đã nhận được nội dung: <i>{noiDung}</i></p>" +
                               $"<p>Đội ngũ hỗ trợ sẽ sớm liên lạc lại qua Mail <b>{email}</b>.</p>" +
                               $"<br/><p>Trân trọng,<br/>PhoneReal Team</p>",
                        IsBodyHtml = true,
                    };
                    mailToCustomer.To.Add(email); // Gửi đến email mà khách đã nhập
                    smtpClient.Send(mailToCustomer);
                }

                return Json(new { success = true, message = "Đã gửi thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi gửi mail: " + ex.Message });
            }
        }
        public IActionResult GioHang()
        {
            // Lấy giỏ hàng từ Session ra để hiển thị
            var cart = GetCartFromSession();
            return View(cart);
        }
        // 1. Hàm Xóa sản phẩm khỏi giỏ
        public IActionResult XoaKhoiGio(int id)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(p => p.ProductId == id);

            if (item != null)
            {
                cart.Remove(item); // Xóa khỏi danh sách
            }

            // Lưu lại giỏ hàng mới vào Session
            HttpContext.Session.SetString("GioHang", JsonSerializer.Serialize(cart));

            // Quay lại trang Giỏ hàng
            return RedirectToAction("GioHang", "Home");
        }

        // 2. Hàm Cập nhật số lượng (khi bấm nút lên/xuống hoặc nhập số)
        public IActionResult CapNhatSoLuong(int id, int soLuong)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(p => p.ProductId == id);

            if (item != null)
            {
                // Nếu số lượng mới > 0 thì cập nhật, ngược lại thì xóa luôn
                if (soLuong > 0)
                {
                    item.SoLuong = soLuong;
                }
                else
                {
                    cart.Remove(item);
                }
            }

            HttpContext.Session.SetString("GioHang", JsonSerializer.Serialize(cart));
            return RedirectToAction("GioHang", "Home");
        }
        private List<Sanphamtronggiohang> GetCartFromSession()
        {
            var session = HttpContext.Session.GetString("GioHang");
            if (string.IsNullOrEmpty(session))
            {
                return new List<Sanphamtronggiohang>();
            }
            return System.Text.Json.JsonSerializer.Deserialize<List<Sanphamtronggiohang>>(session) ?? new List<Sanphamtronggiohang>();
        }
        [Authorize] // 1. Bắt buộc đăng nhập mới được xem
        public IActionResult LichSuDonHang()
        {
            // 2. Lấy Email của người đang đăng nhập
            var userEmail = User.Identity?.Name;

            // 3. Lọc danh sách: Chỉ lấy đơn nào có Email trùng với người dùng
            var danhSachDonHang = _context.DonHangs
                                    .Include(d => d.ChiTietDonHangs)
                                    .Where(d => d.Email == userEmail) // <--- QUAN TRỌNG NHẤT
                                    .OrderByDescending(d => d.NgayDat)
                                    .ToList();

            return View(danhSachDonHang);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiDanhGia(int ProductId, int SoSao, string NoiDung, int? parentId)
        {
            // 1. SỬA LỖI CS8602: Thêm dấu ? và kiểm tra != true
            if (User.Identity?.IsAuthenticated != true)
            {
                return Redirect("/Identity/Account/Login");
            }

            // 2. Lấy thông tin user
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Redirect("/Identity/Account/Login");

            // 3. Tạo đánh giá
            DanhGia danhGiaMoi = new DanhGia
            {
                ProductId = ProductId,
                UserName = user.Email,
                SoSao = SoSao,
                NoiDung = NoiDung,
                NgayDanhGia = DateTime.Now,
                ParentId = parentId
            };

            _context.DanhGias.Add(danhGiaMoi);
            await _context.SaveChangesAsync();

            return RedirectToAction("Chitietsp", new { id = ProductId });
        }
        
        [HttpPost]
        public async Task<IActionResult> LikeDanhGia(int id)
        {
            var danhGia = await _context.DanhGias.FindAsync(id);
            if (danhGia == null) return Json(new { success = false });

            danhGia.LuotThich++; // Tăng 1 like
            await _context.SaveChangesAsync();

            return Json(new { success = true, likes = danhGia.LuotThich });
        }
        // --- CODE XỬ LÝ ĐẶT HÀNG ---
        [HttpPost]
        public async Task<IActionResult> DatHang(string hoTen, string diaChi, string sdt, string email, string ghiChu)
        {
            // 1. Kiểm tra giỏ hàng
            var cart = GetCartFromSession(); // Hàm lấy giỏ hàng của bạn
            if (cart == null || cart.Count == 0) return RedirectToAction("GioHang");

            // 2. Tạo đơn hàng
            var donHang = new DonHang
            {
                NgayDat = DateTime.Now,
                HoTen = hoTen,
                DiaChi = diaChi,
                SoDienThoai = sdt,
                Email = email,
                GhiChu = ghiChu,
                TongTien = cart.Sum(x => x.ThanhTien),
                TrangThai = 0 // 0: Mới đặt
            };
            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync(); // Lưu để lấy được donHang.Id

            // 3. Lưu chi tiết đơn hàng & Trừ kho
            foreach (var item in cart)
            {
                var chiTiet = new ChiTietDonHang
                {
                    DonHangId = donHang.Id, // ID tự sinh sau khi SaveChanges ở bước 2
                    ProductId = item.ProductId,
                    SoLuong = item.SoLuong,
                    DonGia = item.DonGia
                };
                _context.Add(chiTiet);

                // LOGIC TRỪ KHO
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.SoLuongTon -= item.SoLuong;
                    product.DaBan += item.SoLuong; // Xử lý null nếu cần
                    _context.Products.Update(product);
                }
            }
            await _context.SaveChangesAsync();

            // 4. GỬI EMAIL (Phần thêm mới)
            try
            {
                string subject = $"Xác nhận đơn hàng #{donHang.Id} - Web Bán Điện Thoại";
                string body = $@"
                <h3>Cảm ơn {hoTen} đã đặt hàng!</h3>
                <p>Mã đơn hàng của bạn là: <b>#{donHang.Id}</b></p>
                <p>Tổng tiền: <b style='color:red;'>{donHang.TongTien:N0} đ</b></p>
                <p>Hình thức: Thanh toán khi nhận hàng (COD)</p>
                <hr>
                <p><i>Chúng tôi sẽ sớm gửi email thông báo thời gian giao hàng dự kiến. Vui lòng kiểm tra email thường xuyên.</i></p>";

                // Gọi hàm gửi mail (khai báo bên dưới)
                GuiEmail(email, subject, body);
                // --- GỬI CHO ADMIN (Thêm đoạn này) ---
                string emailAdmin = "ngocsummer0309@gmail.com"; // <--- ĐIỀN EMAIL ADMIN VÀO ĐÂY
                string subjectAdmin = $"[ĐƠN MỚI] Khách hàng {hoTen} vừa đặt đơn #{donHang.Id}";
                string bodyAdmin = $@"
        <h3>Có đơn hàng mới cần xử lý!</h3>
        <p><b>Mã đơn:</b> #{donHang.Id}</p>
        <p><b>Khách hàng:</b> {hoTen}</p>
        <p><b>SĐT:</b> {sdt}</p>
        <p><b>Tổng tiền:</b> {donHang.TongTien:N0} đ</p>
        <p><b>Ghi chú:</b> {ghiChu}</p>
        <hr>
        <a href='http://localhost:port/Admin/DonHang'>Bấm vào đây để quản lý đơn hàng</a>";

                GuiEmail(emailAdmin, subjectAdmin, bodyAdmin); // Gửi cho Admin
            }
            catch (Exception ex)
            {
                // Chỉ ghi log lỗi mail, không làm gián đoạn quy trình đặt hàng
                Console.WriteLine("Lỗi gửi mail: " + ex.Message);
            }

            // 5. Xóa giỏ hàng và Chuyển hướng
            HttpContext.Session.Remove("GioHang");

            // QUAN TRỌNG: Chuyển sang Action DatHangThanhCong thay vì Index
            return RedirectToAction("DatHangThanhCong");
        }
        // --- ACTION HIỂN THỊ TRANG THÀNH CÔNG ---
        public IActionResult DatHangThanhCong()
        {
            return View(); // Trả về view DatHangThanhCong.cshtml
        }

        // --- HÀM HỖ TRỢ GỬI MAIL ---
        private void GuiEmail(string toEmail, string subject, string body)
        {
            // CẤU HÌNH GMAIL (Thay đổi thông tin của bạn ở đây)
            var fromEmail = "ngocsummer0309@gmail.com";
            var password = "bylu fthl ykhn ucpg"; // Lấy từ Google App Password

            using (var smtpClient = new SmtpClient("smtp.gmail.com"))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(fromEmail, password);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "Web Điện Thoại Store"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(toEmail);

                smtpClient.Send(mailMessage);
            }
        }
        public IActionResult Lienhe() => View();
        public IActionResult ChinhSach() => View();
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}