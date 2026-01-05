using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webbandt.Models
{
    public class Product
    {
        public int Id { get; set; }

        // Khóa ngoại: Tên cột C# là MaHang.
        public int? MaHang { get; set; }

        // Mối quan hệ: Dùng thuộc tính MaHang làm Khóa ngoại
        [ForeignKey("MaHang")]
        public virtual HangSanXuat? HangSanXuat { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        // Thêm cột Status (Cho phép hiển thị/ngừng bán)
        public bool Status { get; set; } = true;

        // Các thuộc tính khác (đã được sửa thành nullable)
        public string? ManHinh { get; set; }      // Ví dụ: OLED, 6.7 inch
        public string? HeDieuHanh { get; set; }   // Ví dụ: iOS 17
        public string? CameraSau { get; set; }    // Ví dụ: 48MP
        public string? CameraTruoc { get; set; }  // Ví dụ: 12MP
        public string? Chip { get; set; }         // Ví dụ: Apple A17 Pro
        public string? Ram { get; set; }          // Ví dụ: 8GB
        public string? DungLuong { get; set; }    // Ví dụ: 128GB
        public string? Pin { get; set; }          // Ví dụ: 4000mAh
        public int SoLuongTon { get; set; } = 0; // Số lượng còn trong kho
        public int DaBan { get; set; } = 0;      // Số lượng đã bán ra
        public virtual ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
    }
}