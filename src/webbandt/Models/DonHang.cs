using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using webbandt.Data;

namespace webbandt.Models
{
    [Table("DonHang")]
    public class DonHang
    {
        [Key]
        public int Id { get; set; }

        public DateTime NgayDat { get; set; } = DateTime.Now; // Đã khởi tạo để hết lỗi Null

        public decimal TongTien { get; set; }

        public string? HoTen { get; set; } // Thêm dấu ? để cho phép Null (tránh lỗi CS8618)

        public string? SoDienThoai { get; set; }

        public string? Email { get; set; }

        public string? DiaChi { get; set; }

        public string? GhiChu { get; set; }

        public int TrangThai { get; set; }
        // Chỉ khai báo 1 lần duy nhất ở đây và khởi tạo luôn danh sách rỗng
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }
}