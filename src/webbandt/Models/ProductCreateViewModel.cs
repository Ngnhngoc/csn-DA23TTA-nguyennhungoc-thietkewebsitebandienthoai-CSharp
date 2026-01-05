using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using webbandt.Models;

namespace webbandt.Models
{
    public class ProductCreateViewModel
    {
        // 1. Tên sản phẩm: Bỏ 'required', gán mặc định chuỗi rỗng
        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        [Display(Name = "Tên Sản Phẩm")]
        public string Name { get; set; } = string.Empty;

        // 2. Mô tả: Bỏ 'required', dùng [Required]
        [Required(ErrorMessage = "Mô tả không được để trống.")]
        [Display(Name = "Mô Tả Ngắn")]
        public string Description { get; set; } = string.Empty;

        // 3. Giá bán
        [Required(ErrorMessage = "Giá tiền không được để trống.")]
        [Display(Name = "Giá Bán")]
        public double Price { get; set; }

        public bool Status { get; set; } = true;

        // 4. Hãng sản xuất
        [Required(ErrorMessage = "Vui lòng chọn Hãng sản xuất.")]
        [Display(Name = "Hãng Sản Xuất")]
        public int MaHang { get; set; }

        // 5. Ảnh đại diện (QUAN TRỌNG NHẤT ĐỂ SỬA LỖI 400)
        // Bỏ 'required', Thêm dấu '?' (cho phép null tạm thời để Controller kiểm tra sau)
        [Required(ErrorMessage = "Vui lòng chọn ảnh sản phẩm.")]
        [Display(Name = "Ảnh đại diện")]
        public IFormFile? ProductImage { get; set; }

        public int SoLuongTon { get; set; } = 0;

        // --- CÁC THÔNG SỐ KỸ THUẬT (Giữ nguyên) ---
        [Display(Name = "Màn hình")]
        public string? ManHinh { get; set; }

        [Display(Name = "Hệ điều hành")]
        public string? HeDieuHanh { get; set; }

        [Display(Name = "Camera trước")]
        public string? CameraTruoc { get; set; }

        [Display(Name = "Camera sau")]
        public string? CameraSau { get; set; }

        [Display(Name = "Chip xử lý")]
        public string? Chip { get; set; }

        [Display(Name = "RAM")]
        public string? Ram { get; set; }

        [Display(Name = "Dung lượng lưu trữ")]
        public string? DungLuong { get; set; }

        [Display(Name = "Pin & Sạc")]
        public string? Pin { get; set; }
    }
}