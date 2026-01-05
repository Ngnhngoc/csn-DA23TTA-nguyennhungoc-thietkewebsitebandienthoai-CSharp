using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webbandt.Models
{
    [Table("ChiTietDonHang")] // Kết nối với bảng trong SQL
    public class ChiTietDonHang
    {
        [Key]
        public int Id { get; set; }

        public int DonHangId { get; set; } // Khóa ngoại nối về DonHang

        public string? TenSanPham { get; set; } // Thêm dấu ? để tránh lỗi null

        public string? KichThuoc { get; set; }

        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }

        // Mối quan hệ ngược lại (để từ chi tiết truy ngược ra đơn hàng cha)
        [ForeignKey("DonHangId")]
        public virtual DonHang? DonHang { get; set; }

        // QUAN TRỌNG: Đây là dòng bạn đang thiếu, hãy thêm vào ngay
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
        public int ProductId { get; internal set; }
    }
}